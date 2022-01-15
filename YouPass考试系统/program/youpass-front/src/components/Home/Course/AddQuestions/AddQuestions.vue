<template>
  <div class="add-questions">
    <div class="header">
      <span class="back-icon material-icons" @click="triggerBack">
        arrow_back
      </span>
      <div class="text">{{ courseId }}</div>
      /
      <div class="text">AddQuestion</div>
    </div>
    <div class="body">
      <div class="left">
        <div v-for="question in questions" :key="question.id">
          <!-- <transition name="fade" mode="out-in"> -->
          <div v-if="onShow === question.id">
            <AddQuestion
              :triggerErrorToast="triggerErrorToast"
              :triggerGoodToast="triggerGoodToast"
              :courseId="courseId"
              :question="question"
              @questionChanged="handleQuestionChanged($event, question.id)"
            />
          </div>
          <!-- </transition> -->
        </div>
      </div>
      <div class="right">
        <QuestionTable
          :questions="questions"
          :maxQuestionInAView="maxQuestionInAView"
          :onShowGroup="onShowGroup"
          @chooseQuestion="handleChooseQuestion"
          @plus="handlePlus"
          @subtraction="handleSubtraction"
          @goLeft="handleGoLeft"
          @goRight="handleGoRight"
          @submit="handleSubmit"
        />
      </div>
    </div>
  </div>
</template>

<script>
import { ref } from "vue";

import AddQuestion from "./AddQuestion.vue";
import QuestionTable from "./QuestionTable.vue";
import MyPost from "../../../../composables/MyPost";
import BackendPath from "../../../../composables/BackendPath";

export default {
  components: { AddQuestion, QuestionTable },
  props: {
    triggerErrorToast: { type: Function },
    triggerGoodToast: { type: Function },
    courseId: Number,
  },
  emits: ["triggerBack"],
  setup(props, { emit }) {
    const triggerBack = () => {
      emit("triggerBack");
    };
    const handleChooseQuestion = (id) => {
      onShow.value = id;
    };
    const handleQuestionChanged = (question, id) => {
      questions.value[id - 1] = question;
    };
    const handlePlus = () => {
      addQuestion();
    };
    const handleSubtraction = () => {
      // 如果没有内容，下面为空操作
      if (questionNumber.value > 0) {
        questions.value = questions.value.filter(
          (value) => value.id != onShow.value
        );
        questions.value = questions.value.map((value) => {
          if (value.id >= onShow.value) {
            value.id--;
            return value;
          } else {
            return value;
          }
        });
        questionNumber.value--;
        if (onShow.value > 1) {
          onShow.value--;
          onShowGroup.value = Math.floor(
            (onShow.value - 1) / maxQuestionInAView.value
          );
        } else {
          onShow.value = 1;
        }
      }
    };
    const handleGoLeft = () => {
      if (onShowGroup.value > 0) {
        onShowGroup.value--;
      }
      console.log(onShowGroup.value);
    };
    const handleGoRight = () => {
      if (
        (onShowGroup.value + 1) * maxQuestionInAView.value <
        questionNumber.value
      ) {
        onShowGroup.value++;
      }
      console.log(onShowGroup.value);
    };
    // 提交
    const handleSubmit = async () => {
      var isComplete = true;
      questions.value.forEach((value) => {
        var isDone;
        if (value.type === 0) {
          isDone =
            value.subject != "" &&
            value.content != "" &&
            value.option.length > 0 &&
            value.standardAnswer != null;
        } else if (value.type === 1) {
          isDone =
            value.subject != "" &&
            value.content != "" &&
            value.option.length > 0 &&
            value.standardAnswerList.length > 0;
        } else {
          isDone =
            value.subject != "" &&
            value.content != "" &&
            value.fillingAnswer != "";
        }
        if (isDone === false) {
          isComplete = false;
        }
      });
      if (isComplete === false) {
        props.triggerErrorToast("You must complete all the questions");
      } else {
        const data = await MyPost(
          BackendPath + "api/UploadQuestion",
          questions.value.map((value) => {
            var sAnswer = 0;
            var sAnswer_Res = "";
            if (value.type === 0) {
              const optionSize = value.option.length;
              for (var i = 0; i < optionSize; i++) {
                if (i === value.standardAnswer) {
                  sAnswer_Res += "1";
                } else {
                  sAnswer_Res += "0";
                }
              }
            } else if (value.type === 1) {
              const optionSize = value.option.length;
              for (var i = 0; i < optionSize; i++) {
                if (value.standardAnswerList.includes(i)) {
                  sAnswer_Res += "1";
                } else {
                  sAnswer_Res += "0";
                }
              }
              // TODO: 进行了修改
              // value.standardAnswerList.forEach((value) => {
              //   sAnswer = sAnswer | (1 << value);
              //   sAnswer_Res = sAnswer.toString();
              // });
            } else {
              sAnswer_Res = value.fillingAnswer.content;
            }

            var d = new Date();
            var timeStamp = Math.floor(d.getTime() / 1000);
            var option_ret = [];
            value.option.forEach((value) => {
              option_ret.push({ Option_id: value.id, content: value.content });
            });
            var returns = {
              Description: value.content.content,
              Type: value.type,
              Standard_Answer: sAnswer_Res,
              Subject: value.subject,
              Create_time: timeStamp,
              isPrivate: value.isPrivate == true ? 1 : 0,
              option: option_ret,
              // TODO:
              Pool: 0,
              course_id: props.courseId,
            };
            return returns;
          })
        );
        if (data.status != "Good") {
          props.triggerErrorToast("Error");
        } else {
          props.triggerGoodToast("Good");
        }
        emit("triggerBack");
      }
    };

    // 问题总数
    const questionNumber = ref(0);
    // 存问题的列表
    const questions = ref([]);
    // 当前展示的问题
    const onShow = ref(1);
    // 当前展示的问题组
    const onShowGroup = ref(0);
    // 一组最大的内容数
    const maxQuestionInAView = ref(21);
    // 向问题列表添加一条问题
    const addQuestion = () => {
      questionNumber.value++;
      onShow.value = questionNumber.value;
      onShowGroup.value = Math.floor(
        (onShow.value - 1) / maxQuestionInAView.value
      );
      questions.value.push({
        type: 0,
        subject: "",
        isPrivate: false,
        content: { content: "", contentHtml: "" },
        option: [],
        fillingAnswer: { content: "", contentHtml: "" },
        standardAnswer: null,
        standardAnswerList: [],
        // 必须要维护id和这个question在队列中的位置一致
        id: questionNumber.value,
        done: false,
      });
    };
    // 初始状态有一条信息
    addQuestion();

    // test
    // for (var i = 0; i < 40; i++) {
    //   addQuestion();
    // }
    // onShow.value = 1;

    return {
      triggerBack,
      handleChooseQuestion,
      questions,
      onShow,
      onShowGroup,
      maxQuestionInAView,
      handleQuestionChanged,
      handlePlus,
      handleSubtraction,
      handleGoLeft,
      handleGoRight,
      handleSubmit,
    };
  },
};
</script>

<style scoped src="../../../../../public/css/Course.css"></style>
<style scoped>
.body {
  padding: 50px 0;
  font-size: 25px;
  display: flex;
}
.body .left {
  padding-right: 320px;
  width: 100%;
}
.body .right {
  position: fixed;
  top: 0;
  right: 0;
  width: 320px;
  height: 100vh;
  padding: 40px;
  padding-top: 90px;
  /* border: 3px solid crimson; */
}

/* fade transition */
.fade-enter-from {
  opacity: 0;
  /* transform: translateX(50px); */
}
.fade-leave-to {
  opacity: 0;
  /* transform: translateX(-50px); */
}
/* .switch-enter-to,
.switch-leave-from {
  opacity: 1;
  transform: translateY(0);
} */
.fade-enter-active {
  transition: all 0.5s ease;
  display: none;
}
.fade-leave-active {
  transition: all 0.5s ease;
}
</style>
