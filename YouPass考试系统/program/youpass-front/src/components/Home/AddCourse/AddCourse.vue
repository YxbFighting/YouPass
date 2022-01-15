<template>
  <div class="addBlock" v-show="showBlock">
    <Block @submit="submitPassword" @cancel="cancelInput" />
  </div>
  <div class="addcourse">
    <h1>加入课程</h1>
    <div class="search-bar">
      <div class="search-select">
        <select v-model="method">
          <option value="0">课号</option>
          <option value="1">课程名</option>
          <option value="2">老师姓名</option>
        </select>
      </div>

      <input
        class="search-input"
        type="text"
        @keyup.enter="handleSearch"
        v-model="search"
        placeholder="在此输入以搜索课程"
      />
      <div class="search-icon">
        <span class="material-icons" @click="handleSearch"> search </span>
      </div>
    </div>
    <div class="course-list">
      <div
        class="course-card"
        v-for="course in courseList"
        :key="course"
        @click="handleAdd(course.course_id)"
      >
        <div class="title">{{ course.title }}</div>
        <div class="teacher_id">教师编号：{{ course.teacher_id }}</div>
        <div class="course_id">课程编号：{{ course.course_id }}</div>
        <div class="add-button">
          <span class="material-icons"> add </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref } from "vue";
import MyGet from "@/composables/MyGet";
import BackendPath from "@/composables/BackendPath";
import Block from "../AddCourse/Block.vue";
import MyPost from "../../../composables/MyPost";

export default {
  props: {
    triggerErrorToast: { type: Function },
    triggerGoodToast: { type: Function },
  },
  name: "AddCourse",
  components: { Block },

  setup(props) {
    const method = ref("0");
    const search = ref("");

    const courseList = ref([]);

    const triggerErrorToast = props.triggerErrorToast;
    const triggerGoodToast = props.triggerGoodToast;

    const showBlock = ref(false);

    const select = ref("");

    //三种方式搜索课程
    const handleSearch = async () => {
      courseList.value.length = 0;
      let data = ref("");
      if (method.value === "0") {
        data = await MyGet(
          BackendPath + "api/GetCourseById?id=" + search.value
        );
      } else if (method.value === "1") {
        data = await MyGet(
          BackendPath + "api/GetCourseByTitle?title=" + search.value
        );
      } else if (method.value === "2") {
        data = await MyGet(
          BackendPath + "api/GetCourseByTname?name=" + search.value
        );
      }
      if (data.status === "Good") {
        data.info_course.forEach((value, index) => {
          courseList.value.push({
            course_id: value.course_id,
            title: value.title,
            teacher_id: value.teacher_id,
          }); 
        });
        if (courseList.value.length===0) triggerErrorToast("查询无果");
      }
      else triggerErrorToast("");
    };

    // 添加课程
    const handleAdd = (id) => {
      showBlock.value = true;
      select.value = id;
    };
    const submitPassword = async (message) => {
      const data = await MyPost(BackendPath + "api/JoinCourse", {
        course_id: select.value,
        password: message,
      });
      if (data.status === "Good") {
        triggerGoodToast(data.message);
        showBlock.value = false;
      } else triggerErrorToast(data.message);
    };
    const cancelInput = () => {
      showBlock.value = false;
    };

    return {
      method,
      search,
      courseList,
      triggerErrorToast,
      handleSearch,
      handleAdd,
      showBlock,
      cancelInput,
      submitPassword,
    };
  },
};
</script>

<style scoped>
.addcourse {
  width: 100%;
  padding: 40px;
}
.addcourse h1 {
  color: white;
  padding-bottom: 20px;
  border-bottom: 3px solid #bbb;
}
.search-bar {
  display: flex;
  width: 80%;
  padding: 20px 0 0 10px;
  border-radius: 10px;
}
.search-bar .search-select {
  display: flex;
  margin-right: 10px;
}
.search-bar .search-select select {
  width: 110px;
  outline: none;
  background: #dc143c;
  border-radius: 10px;
  text-align-last: center;

  color: #fff;
  border: none;
  font-size: 20px;
  font-family: "Ubuntu", "Ma Shan Zheng", cursive, sans-serif;
}
.search-bar .search-select option {
  background: #ff5353;
}
.search-bar .search-input {
  margin: auto;
  height: 55px;
  font-size: 20px;
  font-family: "Ubuntu", "Ma Shan Zheng", cursive, sans-serif;
}
.search-bar .search-icon {
  display: flex;
  padding-left: 20px;
  height: 50px;
}
.addcourse .material-icons {
  cursor: pointer;
  font-size: 40px;
  line-height: 50px;
}
.search-bar .material-icons:hover {
  color: white;
  transition: all 0.3s ease;
}
.course-list {
  width: 100%;
}
.course-list .course-card {
  display: inline-block;
  width: 300px;
  height: 300px;
  margin: 30px 10px;
  background: #191919;
  border-radius: 20px;
  border-top: 2px solid #c49b3b;
  text-align: center;
  font-size: 30px;
}
.course-list .course-card:hover {
  color: #fff;
  border-top: 2px solid #aaa;
  background: #555;
  cursor: pointer;
  transition: all 0.5s ease;
}
.course-list .course-card .title {
  display: block;
  margin-top: 50px;
  font-size: 30px;
  color: #c49b3b;
  text-align: center;
  overflow: hidden;
}
.course-list .course-card .teacher_id {
  display: block;
  margin: 20px auto;
  text-align: center;
  font-size: 20px;
  color: #fff;
}
.course-list .course-card .course_id {
  display: block;
  margin: 20px auto;
  text-align: center;
  font-size: 20px;
  color: #fff;
}
.course-list .course-card .add-button {
  display: inline-block;
  margin: auto;
  font-size: 50px;
}


</style>