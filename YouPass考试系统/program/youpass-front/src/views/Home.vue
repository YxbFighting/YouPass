<template>
  <div class="home" v-if="isLoaded === true">
    <ToastGroup :showToast="showToast" :isError="isError" :text="text" />
    <div class="navbar">
      <div class="logo">
        <router-link :to="{ name: 'Index' }">You<span>Pass</span></router-link>
      </div>
      <div class="content">
        <div class="menu">
          <div v-if="identity === 0">
            <a
              v-for="item in teacherMenuList"
              :key="item"
              @click="clickMenu($event, item.id)"
              :class="{ onShow: onShow === item.id }"
            >
              <span class="material-icons"> {{ item.icon }} </span
              >{{ item.name }}
            </a>
          </div>
          <div v-else>
            <a
              v-for="item in studentMenuList"
              :key="item"
              @click="clickMenu($event, item.id)"
              :class="{ onShow: onShow === item.id }"
            >
              <span class="material-icons"> {{ item.icon }} </span
              >{{ item.name }}
            </a>
          </div>
        </div>
        <div class="other-function">
          <!-- <a><span class="material-icons"> settings </span>Settings</a> -->
          <a
            v-for="item in ofList"
            :key="item"
            @click="clickMenu($event, item.id)"
            :class="{ onShow: onShow === item.id }"
          >
            <span class="material-icons"> {{ item.icon }} </span>{{ item.name }}
          </a>
        </div>
      </div>
    </div>
    <div class="main">
      <div class="stage">
        <Profile
          :triggerErrorToast="triggerErrorToast"
          :triggerGoodToast="triggerGoodToast"
          v-if="onShow === 0"
        />
        <TakeExam
          :triggerErrorToast="triggerErrorToast"
          :triggerGoodToast="triggerGoodToast"
          v-if="onShow === 1"
        />
        <Course
          :triggerErrorToast="triggerErrorToast"
          :triggerGoodToast="triggerGoodToast"
          :identity="identity"
          v-if="onShow === 2"
        />
        <CreateCourse
          :triggerErrorToast="triggerErrorToast"
          :triggerGoodToast="triggerGoodToast"
          v-if="onShow === 3"
        />
        <AddCourse
          :triggerErrorToast="triggerErrorToast"
          :triggerGoodToast="triggerGoodToast"
          v-if="onShow === 4"
        />
        <Message
          :triggerErrorToast="triggerErrorToast"
          :triggerGoodToast="triggerGoodToast"
          v-if="onShow === 5"
        />
        <LogOut
          :triggerErrorToast="triggerErrorToast"
          :triggerGoodToast="triggerGoodToast"
          v-if="onShow === 6"
          @switchUp="onShow = 0"
        />
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from "vue";
import { useRouter, useRoute } from "vue-router";

import Course from "../components/Home/Course/Course.vue";
import Profile from "../components/Home/Home/Profile";
import TakeExam from "../components/Home/TakeExam/TakeExam";
import LogOut from "../components/Home/Logout/LogOut.vue";
import AddCourse from "../components/Home/AddCourse/AddCourse.vue";
import CreateCourse from "../components/Home/CreateCourse/CreateCourse.vue";
import Message from "../components/Home/Message/Message.vue";
// toast
import getToastHandle from "../composables/GetToastHandle";
import ToastGroup from "../components/ToastGroup";
import MyGet from "../composables/MyGet";
import BackendPath from "../composables/BackendPath";

export default {
  name: "Home",
  components: {
    Profile,
    TakeExam,
    Course,
    ToastGroup,
    LogOut,
    AddCourse,
    CreateCourse,
    Message,
  },
  setup(props) {
    const router = useRouter();
    const route = useRoute();

    const { text, showToast, isError, triggerErrorToast, triggerGoodToast } =
      getToastHandle();

    // 这个人是老师还是学生 老师0 学生1
    const identity = ref(null);

    // 检查是否是合理的请求
    const isLoaded = ref(false);
    onMounted(async () => {
      const data = await MyGet(BackendPath + "api/userinfomation/CheckState");
      if (data === undefined || data.status != "Good") {
        router.push("/notfound");
      }
      const identityData = await MyGet(
        BackendPath + "api/userinfomation/GetIdentity"
      );
      if (identityData === undefined || identityData.status != "Good") {
        router.push("/notfound");
      }
      identity.value = identityData.identity;
      isLoaded.value = true;
    });

    // const menuList = ref([
    //   { icon: "home", name: "Home" },
    //   { icon: "sticky_note_2", name: "Take exam" },
    //   { icon: "connect_without_contact", name: "Course" },
    //   { icon: "create",name: "Create course" },
    //   { icon: "add", name: "Add course"},
    // ]);

    // id:
    // home 0
    // takeexam 1
    // course 2
    // create course 3
    // add course 4
    // message 5
    // logout 6
    const studentMenuList = ref([
      { icon: "home", name: "首页", id: 0 },
      { icon: "sticky_note_2", name: "我的考试", id: 1 },
      { icon: "connect_without_contact", name: "我的课程", id: 2 },
      { icon: "add", name: "加入课程", id: 4 },
    ]);
    const teacherMenuList = ref([
      { icon: "home", name: "首页", id: 0 },
      { icon: "connect_without_contact", name: "我的课程", id: 2 },
      { icon: "create", name: "创建课程", id: 3 },
    ]);

    // other function list
    const ofList = ref([
      { icon: "message", name: "我的消息", id: 5 },
      { icon: "logout", name: "注销", id: 6 },
    ]);

    const onShow = ref(0);

    const clickMenu = (e, id) => {
      onShow.value = id;
    };

    return {
      identity,
      isLoaded,
      studentMenuList,
      teacherMenuList,
      ofList,
      clickMenu,
      onShow,
      // toast
      text,
      showToast,
      isError,
      triggerErrorToast,
      triggerGoodToast,
    };
  },
};
</script>

<style scoped>
.home {
  font-family: "Ubuntu", "Ma Shan Zheng", sans-serif;
  min-width: 1200px;
}
.home .navbar {
  position: fixed;
  height: 100vh;
  background: #191919;
  max-width: 300px;
  min-width: 300px;
  display: block;
  color: #bbb;
  z-index: 3;
}
.navbar .logo {
  text-align: center;
  margin: 40px auto;
  transition: all 0.3s ease;
  /* border: 3px solid crimson; */
}
.navbar .logo a {
  font-size: 50px;
  color: #fff;
  font-weight: 600;
}
.navbar .logo a span {
  color: crimson;
  transition: all 0.3s ease;
}
.navbar .logo:hover {
  cursor: pointer;
  transform: scale(1.2);
}
.navbar .logo a:hover {
  color: crimson;
}
.navbar .menu {
  border-top: 2px solid #252525;
  padding: 30px 0 30px;
}
.navbar .content .other-function {
  border-top: 2px solid #252525;
  border-bottom: 2px solid #252525;
  padding: 30px 0;
}
.navbar .content a {
  margin-top: 15px;
  display: block;
  font-size: 30px;
  font-weight: 100;
  color: #bbb;
  margin-left: 0;
  padding: 2px 14px !important;
  line-height: 20px;
  letter-spacing: 1px;
  border-left: 6px solid #191919;
}
.navbar .content a.onShow {
  border-left: 6px solid crimson;
}
.navbar .content a:hover {
  color: #fff;
  cursor: pointer;
}
.navbar .content a span {
  margin-right: 10px;
}

/* main styling */
.main {
  display: block;
  width: 100%;
  height: auto;
  min-height: 100vh;
  padding-left: 300px;
  background-color: #212121;
  /* border: 3px solid greenyellow; */
}
.main .stage {
  width: 100%;
  margin: 0;
  padding: 0;
  color: #aaa;
}
</style>
