<template>
  <ManageStuent
    v-if="onbuff"
    :courseId="courseId"
    :courseTitle="courseTitle"
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
            offset-sm-1"
        >
          <div class="content-box">
            <h2>Course Info</h2>

            <label class="left">课程ID:</label>
            <label class="right">{{ courseinfo.courseid }}</label>

            <label class="left">课程名称:</label>
            <label class="right">{{ courseinfo.coursetitle }}</label>

            <label class="left">学生数目:</label>
            <label class="right">{{ courseinfo.studentsnum }}</label>

            <button @click="handlemanage">管理学生</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { onMounted, ref } from "vue";
import ManageStuent from "./ViewGradeTeacher/subgroup/ManageStudent.vue";
import MyGet from "../../../composables/MyGet";
import BackendPath from "../../../composables/BackendPath";
export default {
  name: "CourseInfo",
  components: { ManageStuent },
  props: {
    triggerErrorToast: { type: Function },
    triggerGoodToast: { type: Function },
    courseId: String,
    courseTitle: String,
  },
  emits: ["triggerBack"],
  setup(props, { emit }) {
    const triggerBack = () => {
      courseinfo.value.studentsnum = 0;
      emit("triggerBack");
    };
    const courseinfo = ref({
      courseid: props.courseId,
      coursetitle: props.courseTitle,
      studentsnum: 0,
    });
    onMounted(async () => {
      const data = await MyGet(
        BackendPath + "api/Course/StuGet?course_id=" + props.courseId
      );
      console.log(1, data);
      if (data.status === "Good") {
        data.student_list.forEach((value, index) => {
          courseinfo.value.studentsnum += 1;
        });
      }
    });

    //一个课程的所有学生信息：/Course/StuGet    (get请求)
    const handlemanage = () => {
      onbuff.value = true;
    };
    //获取课程信息

    const onbuff = ref(false);
    const handleTrigger = () => {
      onbuff.value = false;
    };
    return {
      onbuff,
      courseinfo,
      triggerBack,
      handlemanage,
      handleTrigger,
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
  padding: 140px 0;
  font-size: 25px;
  align-items: center;
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
}
.content-box .left {
  display: inline-block;
  width: 40%;
}
.content-box .right {
  display: inline-block;
  width: 60%;
}
.content-box label {
  display: inline-block;
  color: #fff;
  font-size: 30px;
  margin: 25px 0 15px 0;
  /* margin: 1em 0 0.8em 0; */
  /* font-weight: 500; */
  font-weight: bold;
  text-transform: uppercase;
  /* letter-spacing: 1px; */
  /* color: #4a4e6d; */
  /* color: #aaa; */
}
.content-box button {
  background: crimson;
  border: 0;
  padding: 15px 35px;
  margin-top: 20px;
  color: white;
  border-radius: 20px;
  cursor: pointer;
  font-size: 1em;
  margin: 1em;
}
.content-box button:hover {
  background: #ff033e;
  transform: scale(1.05);
  cursor: pointer;
}
</style>
