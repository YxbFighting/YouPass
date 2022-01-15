<template>
  <div class="showgrade">
    <div
      class="
        display-port
        col-xl-6
        offset-xl-3
        col-lg-8
        offset-lg-2
        col-sm-10
        offset-sm-1
        text-center
      "
    >
      <span class="back-icon material-icons" @click="triggerBack">
        arrow_back
      </span>
      <div class="content">
        <h2>{{ courseTitle }}-{{ examtitle }}</h2>
        <div class="body">
          <div class="body-head">
            <div class="left">学生编号</div>
            <div class="middle">学生姓名</div>
            <div class="right">学生成绩</div>
          </div>
          <table>
            <!-- <thead>
                         <th>学生编号</th>
                         <th></th>
                         <th>学生</th>
                     </thead> -->
            <tbody>
              <tr v-for="score in scores" :key="score">
                <td>{{ score.studentid }}</td>
                <td>{{ score.studentname }}</td>
                <td>{{ score.studentscore }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { onMounted, ref } from "vue";
import MyGet from "../../../../../composables/MyGet";
import BackendPath from "../../../../../composables/BackendPath";
export default {
  props: {
    courseId: String,
    courseTitle: String,
    examId: String,
    examtitle: String,
  },
  emits: ["triggerBack"],
  setup(props, { emit }) {
    const triggerBack = () => {
      emit("triggerBack");
    };
    const scores = ref([]);
    onMounted(async () => {
      const data = await MyGet(
        BackendPath +
          "api/Course/GradeGet?course_id=" +
          props.courseId +
          "&exam_id=" +
          props.examId
      );
      if (data.status === "Good") {
        data.student_list.forEach((value, index) => {
          scores.value.push({
            studentid: value.stu_id,
            studentname: value.stu_name,
            studentscore: value.stu_score,
          });
        });
      }
    });

    return { triggerBack, scores };
  },
};
</script>

<style scoped>
.showgrade {
  position: fixed;
  z-index: 999;
  top: 0;
  left: 0;
  color: #aaa;
  background: rgba(0, 0, 0, 0.5);
  width: 100vw;
  height: 100vh;
  padding: 30px 0;
}
.showgrade .back-icon {
  margin: auto 30px auto 0;
  font-size: 25px;
  cursor: pointer;
  margin-right: 30px;
  /* transition: all 0.3s ease; */
}

.showgrade .back-icon:hover {
  color: crimson;
  /* transform: scale(1.2); */
}
.showgrade .display-port {
  background: #252525;
  height: 100%;
  border-radius: 20px;
  padding: 50px;
}
.display-port .content h2 {
  text-align: center;
  font-size: 40px;
  color: #fff;
  margin-bottom: 20px;
}

.body {
  height: 550px;
  overflow: scroll;
  width: 100%;
}
.body::-webkit-scrollbar {
  width: 0.05px;
}
.body .body-head {
  display: flex;
}
.body table {
  border-collapse: separate;
  border-spacing: 10px;
  margin: 0 auto;
  margin-top: 30px;
  width: 80%;
  text-align: center;
}
.body table th {
  font-size: 30px;
}
.body table tr {
  font-size: 25px;
}
.body table td {
  margin-left: 0;
  width: 33%;
}
.title {
  display: flex;
  text-align: center;
  align-items: center;
  /* margin:0 auto; */
}
.left {
  text-align: center;
  display: inline-block;
  width: 33%;
  font-size: 35px;
  padding: 10px;
}
.middle {
  text-align: center;
  display: inline-block;
  width: 33%;
  font-size: 35px;
  padding: 10px;
}
.right {
  text-align: center;
  display: inline-block;
  width: 33%;
  font-size: 35px;
  padding: 10px;
}
</style>