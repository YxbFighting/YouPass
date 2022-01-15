<template>
  <ShowExamGrade
    v-if="onshow"
    :courseId="courseId"
    :courseTitle="courseTitle"
    :examId="onexamid"
    :examtitle="onexamtitle"
    @triggerBack="handleTrigger"
  />
  <div class="courseinfot" v-else>
    <div class="header">
      <span class="back-icon material-icons" @click="triggerBack">
        arrow_back
      </span>
      <div class="text">{{ courseId }}</div>
      /
      <div class="text">Course Info</div>
      <!-- else infomation -->
    </div>
    <div class="body-contaner">
      <div class="body">
        <div
          class="
            infostage
            col-xl-8
            offset-xl-2
            col-lg-8
            offset-lg-2
            col-sm-10
            offset-sm-1
          "
        >
          <div class="content-box">
            <h2>考试列表</h2>
            <p>点击列表行以进入学生成绩页面</p>
            <div class="content-table">
              <div class="table_head">
                <div class="left">序号</div>
                <div class="mid">考试编号</div>
                <div class="right">考试名称</div>
              </div>
              <div class="in-table">
                <table>
                  <tbody>
                    <tr
                      v-for="exam in examList"
                      :key="exam"
                      @click="handleclick(exam)"
                      class="choosed"
                    >
                      <td>
                        <div class="text">{{ exam.e_id }}</div>
                      </td>
                      <td>{{ exam.examid }}</td>
                      <td>{{ exam.examtitle }}</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { onMounted, ref } from "vue";
import MyGet from "../../../../composables/MyGet";
import BackendPath from "../../../../composables/BackendPath";
import ShowExamGrade from "./subgroup/ShowExamGrade.vue";
export default {
  props: {
    triggerErrorToast: { type: Function },
    triggerGoodToast: { type: Function },
    courseId: String,
    courseTitle: String,
  },
  components: { ShowExamGrade },
  emits: ["triggerBack"],
  setup(props, { emit }) {
    const triggerBack = () => {
      emit("triggerBack");
    };
    const examList = ref([]);
    const use = ref(1);
    const onshow = ref(false);
    const onexamid = ref("");
    const onexamtitle = ref("");
    onMounted(async () => {
      const data = await MyGet(
        BackendPath + "api/Exam/GetbyCourseId?course_id=" + props.courseId
      );
      console.log(1, data);
      if (data.status === "Good") {
        data.info_Exam.forEach((value, index) => {
          examList.value.push({
            e_id: use.value,
            examid: value.exam_id,
            examtitle: value.title,
          });
          use.value += 1;
        });
      }
    });
    const handleclick = (exam) => {
      onexamid.value = exam.examid;
      onexamtitle.value = exam.examtitle;
      onshow.value = true;
    };
    const handleTrigger = () => {
      onshow.value = false;
    };

    return {
      onshow,
      onexamid,
      onexamtitle,
      triggerBack,
      handleclick,
      examList,
      handleTrigger,
    };
  },
};
</script>

<style scoped src="../../../../../public/css/Course.css"></style>
<style scoped>
@import url("https://fonts.googleapis.com/css2?family=Roboto&display=swap");
*,
*::before,
*::after {
  box-sizing: border-box;
}
.content-box {
  text-align: center;
  align-items: center;
  height: 65vh;
  /* max-height: 65vh; */
}
.content-box .content-table {
  overflow-y: scroll;
  height: 50vh;
}
.content-box .content-table .choosed:hover {
  cursor: pointer;
  color: crimson;
  transition: all 0.3s ease;
}
.content-table::-webkit-scrollbar {
  width: 0.05px;
}
.content-box table tr {
  color: #fff;
}
.content-box table td {
  width: 33%;
  margin: auto 100px;
}
.content-table .table_head {
  display: flex;
  font-size: 30px;
  color: #fff;
}
.content-table .left {
  width: 33%;
  display: inline-block；;
}
.content-table .mid {
  width: 33%;
  display: inline-block；;
}
.content-table .right {
  width: 33%;
  display: inline-block；;
}
.body {
  padding: 140px 0;
  font-size: 25px;
  align-items: center;
  height: 100%;
}

.infostage {
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
  margin-bottom: 20px;
  /* padding:50px; */
}
.content-box p{
  margin-bottom: 10px;
}
.content-box table {
  border-collapse: separate;
  border-spacing: 10px;
  margin: 0 auto;
  margin-top: 30px;
  width: 100%;
  text-align: center;
}
.content-box .in-table {
  overflow-y: scroll;
  height: 50vh;
}
.in-table::-webkit-scrollbar {
  width: 0.05px;
}
</style>