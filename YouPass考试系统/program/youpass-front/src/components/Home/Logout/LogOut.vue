<template>
  <div class="container-box">
    <div class="logout-area">
      <div v-if="text" class="question">{{ text }}</div>
      <div v-else>something error...</div>
      <div class="button-group">
        <button class="answer-yes" @click="handleLogout">Yes</button>
        <button @click="switchToProfile">No</button>
      </div>
    </div>
  </div>
</template>

<script>
import { ref } from "vue";
import { useRouter, useRoute } from "vue-router";

import MyDelete from "@/composables/MyDelete";
import BackendPath from "@/composables/BackendPath";

export default {
  name: "LogOut",
  setup(props, { emit }) {
    // use route and router
    const router = useRouter();
    const route = useRoute();

    const text = ref("");
    const showToast = ref(true);
    const onshow = ref("Home");
    text.value = "Are you sure to log out?";

    const switchToProfile = () => {
      emit("switchUp");
    };

    const handleLogout = async () => {
      await MyDelete(BackendPath + "api/userinfomation");
      router.push({ name: "Login" });
    };

    return { text, showToast, onshow, switchToProfile, handleLogout };
  },
};
</script>

<style scoped>
.container-box {
  /* border: 3px solid crimson;  */
  display: block;
  height: 100vh;
  /* padding-top: 100px; */
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 30px;
}
.logout-area {
  border: 3px solid #fff;
  /* width: 500px; */
  /* text-align: center; */
  padding: 20px;
  color: white;
  background: crimson;
  border-radius: 10px;
  box-shadow: 1px 3px 5px rgba(0, 0, 0, 0.2);
  max-width: 400px;
  min-width: 400px;
  margin: 0 auto;
  height: 150px;
}
.logout-area .question {
  text-align: center;
}
.logout-area .button-group {
  margin-top: 40px;
  display: flex;
  justify-content: space-around;
}
button {
  font-size: 30px;
  color: #fff;
  cursor: pointer;
  background: crimson;
  border: none;
  /* margin-right: 20px; */
}
</style>
