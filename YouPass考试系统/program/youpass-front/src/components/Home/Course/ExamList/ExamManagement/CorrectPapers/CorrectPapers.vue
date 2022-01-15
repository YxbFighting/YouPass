<template>
  <CorrectQuestion
    v-if="onCorrectingQuestionId != null"
    :triggerErrorToast="triggerErrorToast"
    :triggerGoodToast="triggerGoodToast"
    :courseId="courseId"
    :examId="examId"
    :questionId="onCorrectingQuestionId"
    @over="handleOver"
  />
  <div class="correct-papers" v-if="questionList.length">
    <div class="header">
      <span class="back-icon material-icons" @click="triggerBack">
        arrow_back
      </span>
      <div class="text">{{ courseId }}</div>
      /
      <div class="text">考试列表</div>
      /
      <div class="text">考试管理({{ examId }})</div>
      /
      <div class="text">批改试卷</div>
    </div>
    <div class="body-container">
      <div class="body">
        <div class="question-header">
          <div class="left">
            <div class="content-width">题目</div>
            <div class="type-width">类型</div>
            <div class="number-width">人数</div>
          </div>
        </div>
        <div class="question-content">
          <div
            class="question"
            v-for="item in questionList"
            :key="item.id"
            @mouseenter="onHover(item.id)"
            @mouseleave="onLeave(item.id)"
          >
            <div class="left" @click="onHandCorrect(item.id)">
              <div class="content-text content-width">{{ item.content }}</div>
              <div class="type-text type-width">{{ item.type }}</div>
              <div class="number-text number-width">{{ item.number }}</div>
            </div>

            <div
              class="right"
              v-if="hoveredQuestion === item.id"
              @click="onAutoCorrect(item.id)"
            >
              <div class="hint-text">自动批改</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from "vue";

import ListenScroll from "@/composables/ListenScroll";
import CorrectQuestion from "./CorrectQuestion/CorrectQuestion.vue";
import MyGet from "../../../../../../composables/MyGet";
import BackendPath from "../../../../../../composables/BackendPath";
import MyPost from "../../../../../../composables/MyPost";

export default {
  components: { CorrectQuestion },
  props: {
    triggerErrorToast: { type: Function },
    triggerGoodToast: { type: Function },
    courseId: Number,
    examId: Number,
  },
  emits: ["triggerBack"],
  setup(props, { emit }) {
    const triggerBack = () => {
      emit("triggerBack");
    };

    const questionList = ref([]);

    // TODO:
    // 最后要做成通过课程id和考试id获得所有question
    const updateQuestionList = async () => {
      const data = await MyGet(
        BackendPath +
          "api/exam/Getquestions?course_id=" +
          props.courseId +
          "&exam_id=" +
          props.examId
      );
      // console.log(data)
      if (data.status != "Good") {
        props.triggerErrorToast("Error");
        emit("triggerBack");
      } else {
        questionList.value = data.incorrect_list.map((value) => {
          var myType = "";
          if (value.type === 0) {
            myType = "单选";
          } else if (value.type === 1) {
            myType = "多选";
          } else if (value.type === 2) {
            myType = "填空";
          } else {
            myType = "大题";
          }
          return {
            content: value.description,
            type: myType,
            number: value.inCorrectNum,
            id: parseInt(value.question_id),
          };
        });
        if (questionList.value.length === 0) {
          const data = await MyPost(BackendPath + "api/exam/CalStuScore", {
            course_id: props.courseId,
            exam_id: props.examId,
          });
          console.log(data);
          props.triggerGoodToast("恭喜你,你已批改完了");
          emit("triggerBack");
        }
      }
    };
    onMounted(updateQuestionList);

    const hoveredQuestion = ref(null);
    const onHover = (id) => {
      hoveredQuestion.value = id;
    };
    const onLeave = (id) => {
      hoveredQuestion.value = null;
    };
    // 当前正在批改的题目
    const onCorrectingQuestionId = ref(null);
    const onHandCorrect = (id) => {
      onCorrectingQuestionId.value = id;
      console.log(id);
    };
    // 自动批改题目，去调用api
    const onAutoCorrect = async (id) => {
      // console.log(id);
      const data = await MyPost(BackendPath + "api/CorrectingChoice", {
        course_id: props.courseId,
        exam_id: props.examId,
        question_id: id,
      });
      // TODO:
      console.log(data);
      updateQuestionList();
    };
    // 如果子组件传回取消信息
    const handleOver = () => {
      onCorrectingQuestionId.value = null;
      updateQuestionList();
    };

    return {
      triggerBack,
      questionList,
      onHover,
      onLeave,
      hoveredQuestion,
      onCorrectingQuestionId,
      onHandCorrect,
      onAutoCorrect,
      handleOver,
    };
  },
};
</script>

<style scoped src="@/../public/css/Course.css"></style>
<style scoped>
.body {
  padding: 0 40px 20px 40px;
  width: 100%;
}
.body .question-content {
  padding-top: 40px;
}
.body .question {
  border: 1px solid #c49b3b;
  background: #333;
  border-radius: 3px;
  padding: 5px;
  margin: 5px;
  cursor: pointer;
  display: flex;
  /* position: fixed;
  width: 100%; */
}
.body .question:hover {
  background: #555;
  transition: all 0.3s ease;
}
.body .question .content-text {
  /* border: 1px solid #fff; */
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 1; /* number of lines to show */
  -webkit-box-orient: vertical;
  padding: 0 5px;
}
.body .question .type-text {
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 1; /* number of lines to show */
  -webkit-box-orient: vertical;
  text-align: center;
}
.body .question .number-text {
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 1; /* number of lines to show */
  -webkit-box-orient: vertical;
  text-align: center;
}
.body .question .hint-text {
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 1; /* number of lines to show */
  -webkit-box-orient: vertical;
  text-align: center;
  background: #777;
  color: #fff;
  border-radius: 5px;
  transition: all 0.3s ease;
}
.body .question .hint-text:hover {
  color: #fff;
  background: crimson;
}

.body .question-header {
  background: #212121;
  font-size: 30px;
  text-align: center;
  display: flex;
  width: 100%;
  height: 40px;
  transition: all 0s;
  position: fixed;
  padding-left: 340px;
  left: 0;
  top: 50px;
  padding-right: 40px;
  /* border: 1px solid crimson; */
}
.left {
  width: 90%;
  display: flex;
}
.right {
  width: 10%;
}
.content-width {
  width: 60%;
}
.type-width {
  width: 20%;
}
.number-width {
  width: 20%;
}
.hint-width {
  width: 10%;
}
</style>
