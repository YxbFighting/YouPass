<template>
  <div class="exam-page" v-if="onexam" :class="{ gray: onbuff }">
    <div class="question-stage">
      <div class="question-table">
        <h3>选择</h3>
        <div
          v-for="question in getchoices"
          :key="question.id"
          class="question-number"
          :class="{ done: question.done }"
          @click="onShow = question.id"
        >
          {{ question.id }}
        </div>
        <h3>多选</h3>
        <div
          v-for="question in getmultiple"
          :key="question.id"
          class="question-number"
          :class="{ done: question.done }"
          @click="onShow = question.id"
        >
          {{ question.id }}
        </div>
        <h3>填空</h3>
        <div
          v-for="question in getcompletion"
          :key="question.id"
          class="question-number"
          :class="{ done: question.done }"
          @click="onShow = question.id"
        >
          {{ question.id }}
        </div>
        <h3>解答</h3>
        <div
          v-for="question in getcalculation"
          :key="question.id"
          class="question-number"
          :class="{ done: question.done }"
          @click="onShow = question.id"
        >
          {{ question.id }}
        </div>
      </div>
      <div class="examinfo">
        <h1>{{ information.title }}</h1>
        <p>{{ fnum }}/{{ qnum }}</p>
        <p>{{ nowtime }}</p>
        <p>{{ targettime }}</p>
        <button @click="handlesubmit">提交</button>
      </div>
    </div>
    <div class="content-stage">
      <div class="content-container">
        <div v-for="question in questions" :key="question.id">
          <Question
            :question="question"
            v-if="question.id === onShow && isLoaded === true"
            @select="handleSelect($event, question.id, question.dbid)"
            @unselect="handleUnselect($event, question.id, question.dbid)"
            @fillin="handleFillin($event, question.id, question.dbid)"
          />
        </div>
      </div>
    </div>
  </div>
  <div class="buff-page" v-if="onbuff">
    <div class="question-box">
      <div class="content-page">
        <h2>你确定要结束考试</h2>
      </div>
      <button @click="goend">确定</button>
      <button @click="goback">返回</button>
    </div>
  </div>

  <div class="examendplace" v-if="!onexam">
    <div class="question-box">
      <div class="content-page">
        <h2>考试已结束!!!</h2>
      </div>
      <button @click="handleend">返回主页</button>
    </div>
  </div>
</template>

<script>
import { computed, onBeforeUnmount, onMounted, ref, watch } from "vue";
import { useRouter, useRoute } from "vue-router";
import MyGet from "../composables/MyGet.js";
import MyPost from "../composables/MyPost.js";
import Question from "../components/Exam/Question.vue";
import BackendPath from "../composables/BackendPath.js";
import MyDelete from "../composables/MyDelete.js";

export default {
  components: { Question },
  setup(props) {
    const router = useRouter();
    const route = useRoute();
    // id 表示每个题目的题号唯一标识符
    // content 题干
    // options 选择题的选项
    // // 这里的option id 可以改为1 2 3 这样的数字也可以，应该效果更好
    // type 0表示单选 1表示多选 2表示填空 3表示大题
    // done 表示这道题目做了没
    // picked 记录单选的选项
    // pickedList 记录多选的选项
    // written_blank 记录填空的内容
    const questions = ref([]);
    const information = ref({ title: "", starttime: "", endtime: "" });
    const isLoaded = ref(false);
    onMounted(async () => {
      const data = await MyGet(BackendPath + "api/Takeexam");
      console.log(data);
      if (data.status === "Good") {
        information.value.title = data.title;
        information.value.starttime = data.start_time;
        information.value.endtime = data.end_time;

        data.question_list.forEach((value, index) => {
          var tempPicked = [];
          if (value.s_picked != null) {
            tempPicked = value.s_picked;
          }
          var tempPickedList = [];
          if (value.m_picked != null) {
            tempPickedList = value.m_picked;
          }
          questions.value.push({
            id: value.numinpaper,
            dbid: value.id,
            content: value.description,
            type: value.type,
            options: [],
            picked: tempPicked,
            done: value.done,
            pickedList: tempPickedList,
            written_blank: value.fill_content != null ? value.fill_content : "",
          });
        });
        data.question_list.forEach((value, index) => {
          value.options.forEach((v, i) => {
            var tempOptions = {
              option_id: i,
              question_id: v.question_id,
              content: v.content,
            };
            questions.value[index].options.push(tempOptions);
          });
        });
        // console.log(questions.value);
        qnum.value = questions.value.length;
        starttimer();
        isLoaded.value = true;
      } else {
        router.push("/notfound");
      }
    });
    const fnum = ref(0);
    const qnum = ref(0);
    const onShow = ref(1);

    const onexam = ref(true);
    const onbuff = ref(false);

    //考试结束后触发
    const handleend = () => {
      router.push({ name: "Home" });
    };
    //去往结束界面
    const goend = async () => {
      onbuff.value = false;
      onexam.value = false;
      const data = await MyDelete(BackendPath + "api/Takeexam");
    };
    //回到考试界面
    const goback = () => {
      onbuff.value = false;
    };
    //进入buff界面
    const handlesubmit = () => {
      onbuff.value = true;
    };
    //用于实现考试倒计时
    let timer = null;
    const hh = ref("");
    const mf = ref("");
    const ss = ref("");
    const nowtime = ref("");
    const targettime = ref("");
    const starttimer = () => {
      targettime.value = information.value.endtime.substring(11, 19);
      // console.log(targettime.value)
      timer = setInterval(() => {
        hh.value = new Date().getHours();
        mf.value =
          new Date().getMinutes() < 10
            ? "0" + new Date().getMinutes()
            : new Date().getMinutes();
        ss.value =
          new Date().getSeconds() < 10
            ? "0" + new Date().getSeconds()
            : new Date().getSeconds();
        nowtime.value = hh.value + ":" + mf.value + ":" + ss.value;
        if (nowtime.value === targettime.value) {
          clearInterval(timer);
          goend()
        }
      }, 1000);
    };
    //计算属性，用于过滤数组中的项
    const getchoices = computed(() => {
      return questions.value.filter((question) => {
        return question.type ===0;
      });
    });
    const getmultiple = computed(() => {
      return questions.value.filter((question) => {
        return question.type ===1;
      });
    });
    const getcompletion = computed(() => {
      return questions.value.filter((question) => {
        return question.type === 2;
      });
    });
    const getcalculation = computed(() => {
      return questions.value.filter((question) => {
        return question.type === 3;
      });
    });
    // 处理子component 传来的select事件
    const handleSelect = (option_id, q_id, dbid) => {
      questions.value.forEach(async (value, index) => {
        if (value.id === q_id) {
          console.log(1, questions.value[index]);
          if (value.type === 0) {
            questions.value[index].picked = [option_id];
            questions.value[index].done = true;
            console.log(dbid);
            const data = await MyPost(BackendPath + "api/Takeexam/PostAnswer", {
              question_id: dbid,
              fillin_answer: "",
              choice_answer: questions.value[index].picked,
            });
          } else if (value.type === 1) {
            questions.value[index].pickedList.push(option_id);
            questions.value[index].done = true;
            console.log(dbid);
            console.log(questions.value[index].pickedList);
            const data = await MyPost(BackendPath + "api/Takeexam/PostAnswer", {
              question_id: dbid,
              fillin_answer: "",
              choice_answer: questions.value[index].pickedList,
            });
            console.log(data);
          }
          console.log(value.picked, value.pickedList);
        }
      });
    };

    // 处理取消选择
    const handleUnselect = (option_id, q_id, dbid) => {
      questions.value.forEach(async (value, index) => {
        if (value.id === q_id) {
          if (value.type === 0) {
            // error
          } else if (value.type === 1) {
            if (value.pickedList.includes(option_id)) {
              questions.value[index].pickedList = questions.value[
                index
              ].pickedList.filter((pick) => pick != option_id);
              if (questions.value[index].pickedList.length === 0) {
                questions.value[index].done = false;
              }
              const data = await MyPost(
                BackendPath + "api/Takeexam/PostAnswer",
                {
                  question_id: dbid,
                  fillin_answer: "",
                  choice_answer: questions.value[index].pickedList,
                }
              );
            } else {
              // error
            }
          }
        }
      });
    };

    // 处理填空以及大题(子component)
    const handleFillin = async (answer, q_id, dbid) => {
      // console.log(q_id,answer);
      // console.log(questions.value[q_id - 1]);
      questions.value[q_id - 1].written_blank = answer;
      if (answer != "") {
        questions.value[q_id - 1].done = true;
      } else {
        questions.value[q_id - 1].done = false;
      }
      const data = await MyPost(BackendPath + "api/Takeexam/PostAnswer", {
        question_id: dbid,
        fillin_answer: questions.value[q_id - 1].written_blank,
        choice_answer: "",
      });
    };

    // 监听question的变化
    watch(
      () => [..._.cloneDeep(questions.value)],
      (currentValue, oldValue) => {
        fnum.value = 0;
        questions.value.forEach((value, index) => {
          if (value.done === true) {
            fnum.value++;
          }
        });
      }
    );

    return {
      questions,
      targettime,
      nowtime,
      onShow,
      fnum,
      qnum,
      onexam,
      onbuff,
      goback,
      handleSelect,
      handleUnselect,
      handleFillin,
      handleend,
      goend,
      handlesubmit,
      getchoices,
      getmultiple,
      getcompletion,
      getcalculation,
      information,
      isLoaded,
    };
  },
};
</script>

<style scoped>
.exam-page {
  background: #212121;
  min-height: 100vh;
  color: #aaa;
  z-index: -1;
  display: grid;
}
.exam-page.gray {
  transition: 1s;
  filter: brightness(50%);
}
.exam-page .question-stage {
  position: fixed;
  right: 0;
  min-width: 320px;
  max-width: 320px;
  /* width: 300px; */
  height: 100vh;
  padding: 40px;
  /* border: 3px solid crimson; */
}
.exam-page .question-stage .question-table {
  background: #191919;
  height: 70%;
  width: 100%;
  top: 0;
  bottom: 0;
  border-radius: 10px;
  padding: 40px;
  overflow-y: scroll;
  overflow-x: hidden;
  position: relative;
}
/* 滚动条样式设计 */
.exam-page .question-stage .question-table::-webkit-scrollbar {
  width: 4px;
}
.question-table h3 {
  text-align: center;
  font-size: 30px;
  font-family: "Ubuntu", "Ma Shan Zheng", sans-serif;
}
.question-table .question-number {
  display: inline-block;
  background: #aaa;
  font-size: 10px;
  /* padding: auto auto; */
  width: 30px;
  height: 30px;
  line-height: 30px;
  border-radius: 50%;
  text-align: center;
  color: #000;
  margin: 10px;
  cursor: pointer;
  transition: all 0.3s ease;
}
.question-table .question-number.done:hover,
.question-table .question-number:hover {
  background: #fff;
}
.question-table .question-number.done {
  background: #00994c;
}
.question-stage .examinfo {
  position: relative;
  width: 100%;
  height: 20%;
  text-align: center;
  /* border: 3px solid crimson; */
  bottom: 0;
  border-radius: 10px;
  padding: 20px;
}
.examinfo button {
  background: crimson;
  color: white;
  border: 0;
  margin-top: 5px;
  padding: 9px 45px;
  font-size: 1.5em;
  border-radius: 10px;
  transition: all 0.5s;
}
.examinfo button:hover {
  background: #ff033e;
  transform: scale(1.05);
  cursor: pointer;
}
/* conntent styling */
.exam-page .content-stage {
  /* border: 3px solid crimson; */
  padding-right: 320px;
}
.exam-page .content-stage .content-container {
  /* border: 3px solid crimson; */
  min-height: 100vh;
  padding: 100px 0;
}
/* .content-container .content-box {
  width: 1000px;
  border: 3px solid #fff;
  margin: 100px auto;
  background: #c49b3b;
  border-radius: 20px;
  color: #000;
  padding: 40px;
  font-size: 20px;
} */
.examendplace {
  background: #212121;
  min-height: 100vh;
  min-width: 1200px;
  width: 100%;
  padding: 100px 0;
  color: #aaa;
}
.examendplace .question-box {
  font-size: 20px;
  padding: 40px;
  margin: 0 auto;
  width: 750px;
  min-width: 600px;
  text-align: center;
  background: #191919;
  border-radius: 10px;
  border-top: 3px solid #c49b3b;
  color: #aaa;
}
.examendplace .question-box .content-page {
  padding: 50px 50px;
  padding-bottom: 80px;
  /* border:3px solid crimson; */
}
.examendplace .question-box button {
  background: crimson;
  color: white;
  border: 0;
  margin-top: 5px;
  padding: 9px 45px;
  font-size: 1.5em;
  border-radius: 10px;
  transition: all 0.5s;
}
.examendplace .question-box button:hover {
  background: #ff033e;
  transform: scale(1.05);
  cursor: pointer;
}
.buff-page {
  position: fixed;
  z-index: 2000;
  /* border:3px solid crimson; */
  /* visibility: hidden; */
  top: 30%;
  left: 25%;
  color: #aaa;
}
.buff-page:target {
  visibility: visible;
  opacity: 1;
  pointer-events: auto;
}
.buff-page .question-box {
  display: inline-block;
  font-size: 20px;
  padding: 40px;
  margin: 0 auto;
  width: 750px;
  height: 100%;
  min-width: auto;
  text-align: center;
  align-items: center;
  background: #191919;
  border-radius: 10px;
  border-top: 3px solid #c49b3b;
  color: #aaa;
}
.buff-page .question-box .content-page {
  padding: 50px 50px;
  padding-bottom: 80px;
  text-align: center;
}
.buff-page .question-box button {
  background: crimson;
  color: white;
  border: 0;
  margin-top: 5px;
  margin-right: 5px;
  margin-left: 5px;
  padding: 9px 45px;
  font-size: 1.5em;
  border-radius: 10px;
  transition: all 0.5s;
}
.buff-page .question-box button:hover {
  background: #ff033e;
  transform: scale(1.05);
  cursor: pointer;
}
</style>
