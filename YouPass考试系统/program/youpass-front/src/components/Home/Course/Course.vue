<template>
  <!-- 没有id -->
  <Details
    :courseId="detailCourseId"
    :courseTitle="detailCourseTitle"
    :identity="identity"
    :triggerErrorToast="triggerErrorToast"
    :triggerGoodToast="triggerGoodToast"
    @triggerBack="handleBack"
    v-if="showDetail"
  />
  <div class="course-infomation-container" v-else>
    <h1>我的课程</h1>
    <div class="course-card-stage">
      <div
        class="card"
        v-for="course in courseList"
        :key="course"
        @click="handleClick(course.id, course.title)"
      >
        <div class="title">{{ course.title }}</div>
      </div>
    </div>
  </div>
</template>

<script>
import { onMounted, ref } from "vue";

import Details from "./Details.vue";
import MyGet from "../../../composables/MyGet";
import BackendPath from "../../../composables/BackendPath";

export default {
  name: "Course",
  props: {
    triggerErrorToast: { type: Function },
    triggerGoodToast: { type: Function },
    identity: Number,
  },
  components: { Details },
  setup(props) {
    // 记录id
    // const id = props.id;
    const courseList = ref([]);
    onMounted(async () => {
      const data = await MyGet(BackendPath + "api/Course");
      console.log(data);
      if (data.status != "Good") {
        props.triggerErrorToast("Error");
      } else {
        courseList.value = data.info_course.map((value) => {
          return { title: value.title, id: parseInt(value.course_id) };
        });
      }
    });

    // 用于显示细节
    const detailCourseId = ref(null);
    const detailCourseTitle = ref(null);
    const showDetail = ref(false);
    const handleClick = (courseId, courseTitle) => {
      showDetail.value = true;
      detailCourseId.value = courseId;
      console.log(detailCourseId.value);
      detailCourseTitle.value = courseTitle;
    };
    const handleBack = async () => {
      showDetail.value = false;
      detailCourseId.value = null;
      detailCourseTitle.value = null;
      const data = await MyGet(BackendPath + "api/Course");
      console.log(data);
      if (data.status != "Good") {
        props.triggerErrorToast("Error");
      } else {
        courseList.value = data.info_course.map((value) => {
          return { title: value.title, id: parseInt(value.course_id) };
        });
      }
    };
    return {
      courseList,
      handleClick,
      showDetail,
      detailCourseTitle,
      detailCourseId,
      handleBack,
    };
  },
};
</script>

<style scoped>
.course-infomation-container {
  width: 100%;
  padding: 40px;
}
.course-infomation-container h1 {
  color: #fff;
  border-bottom: 3px solid #bbb;
  padding-bottom: 20px;
}
.course-infomation-container .course-card-stage {
  margin-top: 20px;
}
.course-card-stage .card {
  display: inline-block;
  width: 300px;
  height: 300px;
  margin: 10px 10px;
  background: #191919;
  border-radius: 20px;
  border-top: 2px solid #c49b3b;
  cursor: pointer;
  transition: all 0.3s ease;
}
.course-card-stage .card:hover {
  transform: scale(1.05);
}
.course-card-stage .card .title {
  display: block;
  margin: 50px auto;
  font-size: 30px;
  color: #c49b3b;
  text-align: center;
  overflow: hidden;
}
</style>
