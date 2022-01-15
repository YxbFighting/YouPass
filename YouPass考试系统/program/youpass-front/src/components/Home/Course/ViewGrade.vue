<template>
  <div class="viewgrade_vue">
    <div class="header">
      <span class="back-icon material-icons" @click="triggerBack">
        arrow_back
      </span>
      <div class="text">{{ courseId }}</div>
      /
      <div class="text">查看成绩</div>
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
          <div class="content-box">
            <h2>{{ courseTitle }}考试成绩</h2>
            <div class="t">
              <div class="left">考试编号</div>
              <div class="mid">考试名称</div>
              <div class="right">考试成绩</div>
            </div>
            <div class="in-table">
              <table>
                <tbody>
                  <tr v-for="grade in grades" :key="grade">
                    <td>{{ grade.exam_id }}</td>
                    <td>{{ grade.exam_title }}</td>
                    <td v-if="grade.score===-1">无成绩</td>
                    <td v-else>{{ grade.score }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from "vue";
import MyGet from "../../../composables/MyGet";
import BackendPath from "@/composables/BackendPath";

export default {
  name: "ViewGrade",
  props: {
    triggerErrorToast: { type: Function },
    triggerGoodToast: { type: Function },
    courseId: Number,
    courseTitle: String,
  },
  emits: ["triggerBack"],
  setup(props, { emit }) {
    const triggerBack = () => {
      emit("triggerBack");
    };
    const grades = ref([]);
    onMounted(async () => {
      // console.log(props.courseId)
      const data = await MyGet(
        BackendPath + "api/Exam/GetStuScore?" + "course_id=" + props.courseId
      );
      console.log(data);
      data.exam_list.forEach((value, index) => {
        grades.value.push({
          exam_id: value.exam_id,
          exam_title: value.title,
          score: value.score,
        });
      });
    });

    return {
      triggerBack,
      grades,
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
  height: 100%;
  /* align-items: center; */
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
  font-size: 30px;
  color: #fff;
  margin-bottom: 20px;
}
.content-box table {
  border-collapse: separate;
  border-spacing:10px;
  margin:0 auto;
  margin-top:30px;
  width:80%;
  text-align: center;
}
.content-box table tr:hover{
  cursor: pointer;
  /* background: crimson; */
  /* border:3px solid border; */
}
.content-box .t {
  display: flex;
}
.content-box .left {
  width: 33%;
  display: inline-block；;
}
.content-box .mid {
  width: 33%;
  display: inline-block;
}
.content-box .right {
  width: 33%;
  display: inline-block;
}
.in-table{
    overflow-y: scroll;
    height: 50vh;
}
.in-table::-webkit-scrollbar {
  width: 0.05px;
}
</style>
