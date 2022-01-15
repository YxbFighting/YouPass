<template>
  <div
    class="correct-question"
    @click.self="handleCancel"
    v-if="answerList.length"
  >
    <div
      class="correct-question-left col-xl-3 offset-xl-0 col-lg-3 offset-lg-0 col-sm-3 offset-sm-0 text-center"
    >
      <div class="up">
        <div class="description-box">
          <h2>题目信息</h2>
          <div class="information-row">
            <div class="left">
              <h3>题目分数:</h3>
            </div>
            <div class="right">
              {{ question.score }}
            </div>
          </div>
          <div class="information-row">
            <div class="left">
              <h3>题目类型:</h3>
            </div>
            <div class="right">
              <div v-if="question.type === 0">单选</div>
              <div v-else-if="question.type === 1">多选</div>
              <div v-else-if="question.type === 2">填空</div>
              <div v-else>大题</div>
            </div>
          </div>
        </div>
      </div>
      <div class="down">
        <ScoreTable
          :preScore="answerList[onShow - 1].score"
          :maxScore="question.score"
          @next="handlechange"
          @scoreChanged="handleScoreChange"
        />
      </div>
    </div>
    <div class="correct-question-mid col-xl-9 col-lg-9 col-sm-9 text-center">
      <div class="question-box">
        <div class="question-content">
          <h2>题干</h2>
          <div class="content-text" v-html="question.contentHtml"></div>
          <h2>标准答案</h2>
          <div class="standard-answer">
            <div v-if="question.type === 0">
              <div
                class="choice-question"
                v-for="item in question.option"
                :key="item"
                :class="{ picked: item.id === question.standardAnswer }"
                v-html="item.contentHtml"
              ></div>
            </div>
            <div v-else-if="question.type === 1">
              <div
                class="choice-question"
                v-for="item in question.option"
                :key="item"
                :class="{
                  picked: question.standardAnswerList.includes(item.id),
                }"
                v-html="item.contentHtml"
              ></div>
            </div>
            <div v-else>
              <div class="other-question">
                {{ question.standardFillingAnswer }}
              </div>
            </div>
          </div>
        </div>
        <div class="question-answer">
          <h2>学生{{ answer.id }}答案</h2>
          <div class="question-answer-content">
            <div v-if="question.type === 0">
              <div
                class="choice-question"
                v-for="item in question.option"
                :key="item"
                :class="{ picked: item.id === answer.answer }"
              >
                {{ item.content }}
              </div>
            </div>
            <div v-else-if="question.type === 1">
              <div
                class="choice-question"
                v-for="item in question.option"
                :key="item"
                :class="{ picked: answer.answerList.includes(item.id) }"
              >
                {{ item.content }}
              </div>
            </div>
            <div v-else>
              <div class="other-question">
                {{ answer.fillingAnswer }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="correct-question-right col-xl-3 col-lg-3 col-sm-3 text-center">
      <StudentTable
        :answerList="answerList"
        :maxQuestionInAView="maxQuestionInAView"
        :onShowGroup="onShowGroup"
        @chooseQuestion="handleChooseQuestion"
        @goLeft="handleGoLeft"
        @goRight="handleGoRight"
        @submit="handleSubmit"
        @cancel="handleCancel"
      />
    </div>
  </div>
</template>

<script>
import { ref, getCurrentInstance, onMounted } from "vue";
import StudentTable from "./StudentTable.vue";
import ScoreTable from "./ScoreTable.vue";
import MyPost from "@/composables/MyPost";
import BackendPath from "@/composables/BackendPath";
import Parse from "../../../../../../../composables/LatexParse";

export default {
  components: { StudentTable, ScoreTable },
  props: {
    triggerErrorToast: { type: Function },
    triggerGoodToast: { type: Function },
    courseId: Number,
    examId: Number,
    questionId: Number,
  },
  emits: ["over"],
  setup(props, { emit }) {
    // latex 支持
    const { ctx: _this } = getCurrentInstance();
    const reRender = () => {
      _this.$forceUpdate();
      MathJax.Hub.Typeset();
    };

    // 考题信息
    const question = ref({});
    // const question = ref({
    //   content: "123",
    //   option: [
    //     { content: "123123", id: 0 },
    //     {
    //       content:
    //         "2121323123123123333333333333333333333333333333333321213231231231233333333333333333333333333333333333",
    //       id: 1,
    //     },
    //     { content: "31213123", id: 2 },
    //     { content: "4123123", id: 3 },
    //   ],
    //   type: 1,
    //   standardAnswer: 1,
    //   standardAnswerList: [1, 0],
    //   standardFillingAnswer: "1231231231",
    //   //other information...
    //   score: 4,
    // });

    // 学生答案信息
    const answerList = ref([]);
    // for (var i = 1; i <= 40; i++) {
    //   answerList.value.push({
    //     s_id: "123",
    //     answer: 1,
    //     answerList: [1, 2],
    //     fillingAnswer: "3322",
    //     id: i,
    //     done: false,
    //     score: null,
    //   });
    // }
    // answerList.value[0].answerList = [1];

    onMounted(async () => {
      const data = await MyPost(BackendPath + "api/GetStudofQues", {
        course_id: props.courseId,
        exam_id: props.examId,
        question_id: props.questionId,
      });
      console.log(data);
      if (data.status != "Good") {
        props.triggerErrorToast("Error");
      } else if (data.student_list.length === 0) {
        emit("over");
      } else {
        // 处理question
        const question_info = data.question_info;
        const tempOption = [];
        question_info.options.forEach((value, index) => {
          tempOption.push({
            content: value.content,
            id: value.option_id,
            contentHtml: Parse(value.content),
          });
        });
        var tempStandardAnswer = -1;
        var tempStandardAnswerList = [];
        var tempStandardFillingAnswer = "";
        if (question_info.type === 0) {
          const strSize = question_info.standard_answer.length;
          for (var i = 0; i < strSize; i++) {
            if (question_info.standard_answer[i] == "1") {
              tempStandardAnswer = i;
            }
          }
        } else if (question_info.type === 1) {
          const strSize = question_info.standard_answer.length;
          for (var i = 0; i < strSize; i++) {
            if (question_info.standard_answer[i] == "1") {
              tempStandardAnswerList.push(i);
            }
          }
        } else {
          tempStandardFillingAnswer = question_info.standard_answer;
        }
        // question结果
        question.value = {
          content: question_info.description,
          contentHtml: Parse(question_info.description),
          option: tempOption,
          type: parseInt(question_info.type),
          standardAnswer: tempStandardAnswer,
          standardAnswerList: tempStandardAnswerList,
          standardFillingAnswer: tempStandardFillingAnswer,
          score: parseInt(question_info.ques_value),
        };
        //学生选项;
        data.student_list.forEach((value, index) => {
          var tempStandardAnswer = -1;
          var tempStandardAnswerList = [];
          var tempStandardFillingAnswer = "";
          if (question.value.type === 0) {
            // 单选
            const strSize = value.student_answer.length;
            for (var i = 0; i < strSize; i++) {
              if (value.student_answer[i] == "1") {
                tempStandardAnswer = i;
              }
            }
          } else if (question.value.type === 1) {
            // 多选
            const strSize = value.student_answer.length;
            for (var i = 0; i < strSize; i++) {
              if (value.student_answer[i] == "1") {
                tempStandardAnswerList.push(i);
              }
            }
          } else {
            tempStandardFillingAnswer = value.student_answer;
          }
          answerList.value.push({
            s_id: value.stud_id,
            answer: tempStandardAnswer,
            answerList: tempStandardAnswerList,
            fillingAnswer: tempStandardFillingAnswer,
            id: index + 1,
            done: false,
            score: null,
          });
          answer.value = answerList.value[0];
        });
        questionNumber.value = answerList.value.length;
        console.log(question.value, answerList.value);
      }
      setTimeout(reRender, 10);
    });

    const answer = ref();
    // 一组最大的内容数
    const maxQuestionInAView = ref(21);
    // 当前展示的问题组
    const onShowGroup = ref(0);
    // 当前展示的问题
    const onShow = ref(1);
    // 问题总数
    const questionNumber = ref(answerList.value.length);
    const handleChooseQuestion = (id) => {
      onShow.value = id;
      answer.value = answerList.value.filter((value) => value.id === id)[0];
    };
    const handleGoLeft = () => {
      console.log(onShowGroup.value);
      if (onShowGroup.value > 0) {
        onShowGroup.value--;
      }
    };
    const handleGoRight = () => {
      console.log(questionNumber.value);
      if (
        (onShowGroup.value + 1) * maxQuestionInAView.value <
        questionNumber.value
      ) {
        onShowGroup.value++;
      }
    };
    const handleSubmit = async () => {
      // 调用api
      var scoreList = [];
      answerList.value.forEach((value) => {
        if (value.done === true) {
          scoreList.push({ ID: value.s_id, score: value.score });
        }
      });
      const data = await MyPost(
        BackendPath + "api/CorrectingChoice/ManualCorrectingPost",
        {
          course_id: props.courseId,
          exam_id: props.examId,
          question_id: props.questionId,
          student_score: scoreList,
        }
      );
      if (data.status != "Good") {
        props.triggerErrorToast("Error");
      } else {
        props.triggerGoodToast("Success");
      }
      emit("over");
    };
    const handleCancel = () => {
      emit("over");
    };
    //点击左下角切换学生后触发的函数
    const handlechange = () => {
      if (onShow.value < questionNumber.value) {
        onShow.value++;
        answer.value = answerList.value.filter(
          (value) => value.id === onShow.value
        )[0];
      }
    };

    const handleScoreChange = (s) => {
      if (s != answerList.value[onShow.value - 1].score) {
        answerList.value[onShow.value - 1].score = s;
        answerList.value[onShow.value - 1].done = true;
      }
    };

    return {
      question,
      answer,
      answerList,
      maxQuestionInAView,
      handleChooseQuestion,
      onShowGroup,
      handleGoLeft,
      handleGoRight,
      handleSubmit,
      handleCancel,
      handlechange,
      handleScoreChange,
      onShow,
    };
  },
};
</script>

<style scoped>
.correct-question {
  position: fixed;
  z-index: 999;
  top: 0;
  left: 0;
  background: rgba(255, 255, 255, 0.5);
  width: 100vw;
  height: 100vh;
  display: flex;
  font-family: "Ubuntu", "Ma Shan Zheng", sans-serif;
}
.correct-question .correct-question-left {
  display: flex;
  flex-direction: column;
  /* border: 3px solid #000; */
}
.correct-question .correct-question-mid {
  /* display: inline-block; */
  /* height: inherit; */
  /* border: 3px solid #000; */
  padding: 5px 330px 5px 10px;
}
.correct-question .correct-question-right {
  /* display: inline-block; */
  /* height: inherit; */
  /* border: 3px solid #000; */
  position: fixed;
  top: 0;
  right: 0;
  width: 320px;
  height: 100vh;
  padding: 40px;
  padding-top: 90px;
}

.correct-question .correct-question-left .up {
  height: 60%;
  padding: 10px;
  /* margin-top: 60%; */
}
.correct-question .correct-question-left .up .information-row {
  margin-top: 5px;
  padding: 0 5px;
}
.correct-question .correct-question-left .up .information-row h3 {
  color: #fff;
  font-size: 25px;
}
.correct-question .correct-question-left .up .left {
  display: inline-block;
  width: 50%;
}
.correct-question .correct-question-left .up .right {
  display: inline-block;
  width: 50%;
}
.correct-question .correct-question-left .down {
  padding: 10px;
  height: 40%;
}

.correct-question-mid .question-box {
  height: 100%;
  padding: 10px;
  color: #aaa;
  /* border: 3px solid crimson; */
}
.correct-question-mid h2 {
  font-size: 30px;
  text-align: center;
  color: #fff;
}
.correct-question-mid .question-box .question-content {
  height: 60%;
  padding: 20px;
  /* border-bottom: 1px dashed #c49b3b; */
  border-radius: 10px;
  /* border-top: 3px solid #c49b3b; */
  overflow-y: scroll;
  background: #252525;
  /* overflow: scroll; */
  /* border: 1px solid crimson; */
}
.correct-question-mid .question-box .question-answer {
  height: 40%;
  padding: 10px;
  margin-top: 2px;
  border-radius: 10px;
  /* border-top: 3px solid #c49b3b; */
  overflow-y: scroll;
  background: #252525;
  /* border: 1px solid #000; */
  /* overflow: scroll; */
}

.choice-question {
  margin: 20px auto;
  padding: 10px 20px;
  background: #191919;
  border-radius: 10px;
  border-left: 20px solid #aaa;
  color: #aaa;
  word-break: break-all;
  transition: all 0.3s ease;
}
.choice-question.picked {
  border-left: 20px solid #00994c;
}

.correct-question .correct-question-left .up .description-box {
  background: #252525;
  height: 100%;
  border-radius: 10px;
}
.correct-question .correct-question-left .up .description-box h2 {
  font-size: 30px;
  text-align: center;
  color: #fff;
}
</style>
