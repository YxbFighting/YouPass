<template>
  <div class="release_vue">
    <div class="header">
      <span class="back-icon material-icons" @click="triggerBack">
        arrow_back
      </span>
      <div class="text">{{ courseId }}</div>
      /
      <div class="text">发布考试</div>
      <!-- else infomation -->
    </div>
    <div class="body-container">
      <div class="body">
        <div
          class="
            question-stage
            col-xl-8
            offset-xl-2
            col-lg-8
            offset-lg-2
            col-sm-10
            offset-sm-1
          "
        >
          <div class="content-box" v-if="showtable === 'examsubmit'">
            <h2>发布考试</h2>

            <form @submit.prevent="handleSubmit">
              <label class="left">课程名:</label>
              <div class="right title">{{ courseTitle }}</div>

              <label class="left">考试名称:</label>
              <div class="right">
                <input
                  class="ctitle"
                  type="text"
                  required
                  v-model="examTitle"
                />
              </div>

              <label class="left">考试日期:</label>
              <div class="right">
                <form>
                  <input class="date" type="date" v-model="examDate" />
                </form>
              </div>
              <div v-if="dateerror" class="error">
                {{ dateerror }}
              </div>

              <label class="left">开始时间:</label>
              <div class="right">
                <input
                  class="time"
                  type="time"
                  required
                  v-model="examStarttime"
                />
              </div>

              <label class="left">结束时间:</label>
              <div class="right">
                <input
                  class="time"
                  type="time"
                  required
                  v-model="examEndtime"
                />
              </div>
              <div v-if="endtimeerror" class="error">
                {{ endtimeerror }}
              </div>

              <label class="bigtitle">题目信息设置</label>
              <div v-if="questionnumerror" class="error">
                {{ questionnumerror }}
              </div>

              <div class="q">
                <label class="tleft">题目类型</label>
                <label class="tmid">题目数量</label>
                <label class="tright">题目分数</label>

                <label class="tleft">单选题:</label>
                <div class="tmid">
                  <input
                    class="num"
                    type="number"
                    min="0"
                    oninput="this.value=this.value.replace(/\D/g);"
                    required
                    v-model="examChoicenum"
                  />
                </div>
                <div class="tright">
                  <input
                    class="num"
                    type="number"
                    min="0"
                    oninput="this.value=this.value.replace(/\D/g);"
                    required
                    v-model="examChoicescore"
                  />
                </div>
                <div v-if="choiceerror" class="error">
                  {{ choiceerror }}
                </div>

                <label class="tleft">多选题:</label>
                <div class="tmid">
                  <input
                    class="num"
                    type="number"
                    min="0"
                    oninput="this.value=this.value.replace(/\D/g);"
                    required
                    v-model="examMultiplenum"
                  />
                </div>
                <div class="tright">
                  <input
                    class="num"
                    type="number"
                    min="0"
                    oninput="this.value=this.value.replace(/\D/g);"
                    required
                    v-model="examMultiplescore"
                  />
                </div>
                <div v-if="multipleerror" class="error">
                  {{ multipleerror }}
                </div>

                <label class="tleft">填空题:</label>
                <div class="tmid">
                  <input
                    class="num"
                    type="number"
                    min="0"
                    oninput="this.value=this.value.replace(/\D/g);"
                    required
                    v-model="examCompletionnum"
                  />
                </div>
                <div class="tright">
                  <input
                    class="num"
                    type="number"
                    min="0"
                    oninput="this.value=this.value.replace(/\D/g);"
                    required
                    v-model="examCompletionscore"
                  />
                </div>
                <div v-if="completionerror" class="error">
                  {{ completionerror }}
                </div>

                <label class="tleft">解答题:</label>
                <div class="tmid">
                  <input
                    class="num"
                    type="number"
                    min="0"
                    oninput="this.value=this.value.replace(/\D/g);"
                    required
                    v-model="examWrittennum"
                  />
                </div>
                <div class="tright">
                  <input
                    class="num"
                    type="number"
                    min="0"
                    oninput="this.value=this.value.replace(/\D/g);"
                    required
                    v-model="examWrittenscore"
                  />
                </div>
                <div v-if="writtenerror" class="error">
                  {{ writtenerror }}
                </div>
              </div>

              <div class="submit">
                <button>提交</button>
              </div>
            </form>
          </div>
          <div class="content-box" v-if="showtable == 'right'">
            <h2>考试添加成功!</h2>
            <h3>考试的编号为: {{ examid }}</h3>
            <button @click="triggerBack" class="rightbutton">返回</button>
          </div>
          <div class="content-box" v-if="showtable == 'wrong'">
            <h2>{{ dataerror }}</h2>
            <button @click="triggerBack" class="rightbutton">返回</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref ,watch} from "vue";
import MyPost from "../../../composables/MyPost";
import BackendPath from "../../../composables/BackendPath";
export default {
  name: "ReleaseTest",
  props: {
    triggerErrorToast: { type: Function },
    triggerGoodToast: { type: Function },
    courseId: String,
    courseTitle: String,
  },
  emits: ["triggerBack"],
  setup(props, { emit }) {
    const triggerBack = () => {
      emit("triggerBack");
    };
    // console.log(props.persenId)
    // console.log(1,props.courseId)
    const iserror = ref(false);
    const dateerror = ref("");
    const endtimeerror = ref("");
    const questionnumerror = ref("");
    const choiceerror = ref("");
    const multipleerror = ref("");
    const completionerror = ref("");
    const writtenerror = ref("");

    const examTitle = ref(null);
    const examDate = ref(null);
    const examStarttime = ref(null);
    const examEndtime = ref(null);
    const examChoicenum = ref(0);
    const examMultiplenum = ref(0);
    const examCompletionnum = ref(0);
    const examWrittennum = ref(0);
    const showtable = ref("examsubmit");
    const dataerror = ref("");
    const examid = ref("");

    const examChoicescore = ref(0);
    const examMultiplescore = ref(0);
    const examCompletionscore = ref(0);
    const examWrittenscore = ref(0);

    const maxNumber = ref(999);
    //watch
    watch(examChoicenum, () => {
      if (examChoicenum.value > maxNumber.value) {
        examChoicenum.value = maxNumber.value;
      }
    });
    watch(examMultiplenum, () => {
      if (examMultiplenum.value > maxNumber.value) {
        examMultiplenum.value = maxNumber.value;
      }
    });
    watch(examCompletionnum, () => {
      if (examCompletionnum.value > maxNumber.value) {
        examCompletionnum.value = maxNumber.value;
      }
    });
    watch(examWrittennum, () => {
      if (examWrittennum.value > maxNumber.value) {
        examWrittennum.value = maxNumber.value;
      }
    });

    watch(examChoicescore, () => {
      if(examChoicescore.value > maxNumber.value) {
        examChoicescore.value = maxNumber.value;
      }
    });
    watch(examMultiplescore, () => {
      if (examMultiplescore.value > maxNumber.value) {
        examMultiplescore.value = maxNumber.value;
      }
    });
    watch(examCompletionscore, () => {
      if (examCompletionscore.value > maxNumber.value) {
        examCompletionscore.value = maxNumber.value;
      }
    });
    watch(examWrittenscore, () => {
      if  (examWrittenscore.value > maxNumber.value) {
        examWrittenscore.value = maxNumber.value;
      }
    });

    // showtable.value='right'
    //获取当前时间的工具函数
    const getcurrentday = () => {
      var date = new Date();
      var seperator1 = "-";
      var year = date.getFullYear();
      var month = date.getMonth() + 1;
      var strDate = date.getDate();
      if (month >= 1 && month <= 9) {
        month = "0" + month;
      }
      if (strDate >= 0 && strDate <= 9) {
        strDate = "0" + strDate;
      }
      var currentdate = year + seperator1 + month + seperator1 + strDate;
      return currentdate;
    };

    const handleSubmit = async () => {
      iserror.value = false;
      if (examDate.value == null) {
        dateerror.value = "you must have a date";
        iserror.value = true;
      } else if (examDate.value < getcurrentday()) {
        dateerror.value = "you can just set exam after today!";
        iserror.value = true;
      } else {
        dateerror.value = "";
      }

      if (examEndtime.value <= examStarttime.value) {
        endtimeerror.value = "end time must be greater than the start time!";
        iserror.value = true;
      } else {
        endtimeerror.value = "";
      }

      if (
        examChoicenum.value == 0 &&
        examCompletionnum.value == 0 &&
        examMultiplenum.value == 0 &&
        examWrittennum.value == 0
      ) {
        questionnumerror.value = "you must add a question";
        iserror.value = true;
      } else {
        questionnumerror.value = "";
        if (examChoicenum.value < 0) {
          choiceerror.value = "choice num must >=0";
          iserror.value = true;
        } else {
          choiceerror.value = "";
        }

        if (examMultiplenum.value < 0) {
          multipleerror.value = "multiple num must >=0";
          iserror.value = true;
        } else {
          multipleerror.value = "";
        }

        if (examCompletionnum.value < 0) {
          completionerror.value = "completion num must >=0";
          iserror.value = true;
        } else {
          completionerror.value = "";
        }

        if (examWrittennum.value < 0) {
          writtenerror.value = "written num must >=0";
          iserror.value = true;
        } else {
          writtenerror.value = "";
        }
      }
      var coursedd = props.courseId;
      var a = ref(examDate.value + " " + examStarttime.value + ":00");
      var b = ref(examDate.value + " " + examEndtime.value + ":00");
      console.log(1, a, b);
      if (!iserror.value) {
        // console.log("right");
        const data = await MyPost(BackendPath + "api/CreateExam", {
          course_Id: coursedd,
          exam_title: examTitle.value,
          exam_starttime: a.value,
          exam_endtime: b.value,
          choice: parseInt(examChoicenum.value),
          multiplechoice: parseInt(examMultiplenum.value),
          completion: parseInt(examCompletionnum.value),
          written: parseInt(examWrittennum.value),
          choicevalue:parseInt(examChoicescore.value),
          multiplechoicevalue:parseInt(examMultiplescore.value),  
          completionvalue:parseInt(examCompletionscore.value),
          writtenvalue:parseInt(examWrittenscore.value)

          // teacher_Id: "10515",
          // course_Id: "44211",
          // exam_title: "数据结构考试13",
          // exam_starttime: "2021-6-22 00:00:00",
          // exam_endtime: "2021-6-22 00:01:00",
          // choice: 1,
          // multiplechoice: 1,
          // completion: 0,
          // written: 1,
        });
        console.log(data);
        if (data.status == "Good") {
          examid.value = data.exam_id;
          showtable.value = "right";
        } else if (data.status == "bad") {
          dataerror.value = data.errorinfo;
          showtable.value = "wrong";
        }
      }
    };

    return {
      triggerBack,
      examTitle,
      examDate,
      examStarttime,
      examEndtime,
      examChoicenum,
      examCompletionnum,
      examWrittennum,
      examMultiplenum,

      examChoicescore,
      examMultiplescore,
      examCompletionscore,
      examWrittenscore,

      dateerror,
      endtimeerror,
      multipleerror,
      choiceerror,
      completionerror,
      writtenerror,
      questionnumerror,

      handleSubmit,
      showtable,
      dataerror,
      examid,
    };
  },
};
</script>

<style scoped src="../../../../public/css/Course.css"></style>
<style scoped>
@import url("https://fonts.googleapis.com/css2?family=Roboto&display=swap");
*,
*::before,
*::after {
  box-sizing: border-box;
}
.content-box {
  text-align: center;
}
.body {
  padding: 40px 0;
  font-size: 25px;
}
.question-stage {
  display: block;
  padding: 40px;
  background: #191919;
  border-radius: 10px;
  border-top: 3px solid #c49b3b;
}

.content-box h2 {
  text-align: center;
  font-size: 40px;
  color: #fff;
}
.content-box .left {
  display: inline-block;
  width: 40%;
}
.content-box .right {
  display: inline-block;
  width: 60%;
}
.content-box .tleft {
  display: inline-block;
  width: 33%;
}
.content-box .tmid {
  display: inline-block;
  width: 33%;
}
.content-box .tright {
  display: inline-block;
  width: 33%;
}
.content-box .ctitle {
  font-size: 25px;
  width: 70%;
  font-weight: bold;
}
.content-box .title {
  color: #fff;
  font-size: 30px;
}
.content-box .bigtitle {
  display: inline-block;
  width: 100%;
  text-align: center;
}
.content-box label {
  display: inline-block;
  color: #fff;
  font-size: 30px;
  margin: 25px 0 15px 0;
  /* margin: 1em 0 0.8em 0; */
  /* font-weight: 500; */
  font-weight: 30px;
  text-transform: uppercase;
  /* letter-spacing: 1px; */
  /* color: #4a4e6d; */
  /* color: #aaa; */
}

.content-box button {
  background: crimson;
  border: 0;
  padding: 10px 20px;
  margin-top: 20px;
  color: white;
  border-radius: 20px;
  cursor: pointer;
  font-size: 0.5em;
  margin: 1em;
}
.submit {
  text-align: center;
  transition: all 0.3s ease;
}
.submit:hover {
  transform: scale(1.05);
}
.content-box .date {
  width: 70%;
  font-size: 25px;
  font-weight: bold;
  /* background: transparent; */
  border: none;
  /* color: white; */
}
.content-box .time {
  width: 70%;
  font-size: 25px;
  font-weight: bold;
  /* background: transparent; */
  border: none;
  /* color: white; */
}
.content-box .num {
  width: 35%;
  font-size: 25px;
  font-weight: bold;
  border: none;
}
.error {
  color: #ff0062;
  margin-top: 10px;
  font-size: 0.4em;
  font-weight: bold;
}

.content-box .rightbutton {
  margin-top: 50px;
  font-size: 25px;
  font-weight: 25px;
}
</style>
