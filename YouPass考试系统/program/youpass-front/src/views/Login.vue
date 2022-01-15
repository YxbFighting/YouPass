<template>
  <div v-if="isLoaded">
    <div class="login-page">
      <ToastGroup :showToast="showToast" :isError="isError" :text="text" />
      <div class="navbar">
        <div class="max-width">
          <div class="logo">
            <router-link :to="{ name: 'Index' }">
              You
              <span>Pass</span>
            </router-link>
          </div>
        </div>
      </div>
      <div class="animation-area">
        <div class="box-area">
          <li></li>
          <li></li>
          <li></li>
          <li></li>
          <li></li>
          <li></li>
        </div>
      </div>

      <div class="layout">
        <div class="container">
          <div class="row">
            <div
              class="col-xl-6 offset-xl-3 col-lg-8 offset-lg-2 col-sm-10 offset-sm-1 text-center"
            >
              <transition name="fade" appear>
                <div class="content-box">
                  <h2>
                    Log in to
                    <span>YouPass</span>
                  </h2>
                  <form @submit.prevent="handleSubmit">
                    <label>Id</label>
                    <input type="id" v-model="id" />
                    <div v-if="idError" class="error">{{ idError }}</div>

                    <label>Password</label>
                    <input type="password" v-model="password" />
                    <div v-if="passwordError" class="error">
                      {{ passwordError }}
                    </div>

                    <div class="submit">
                      <button>Log in</button>
                    </div>
                  </form>
                </div>
              </transition>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { onMounted, ref } from "vue";
import { useRouter, useRoute } from "vue-router";

import getToastHandle from "../composables/GetToastHandle";
import ToastGroup from "../components/ToastGroup";
import MyGet from "../composables/MyGet";
import MyPost from "../composables/MyPost";
import BackendPath from "../composables/BackendPath";

export default {
  name: "Login",
  components: { ToastGroup },
  setup(props) {
    const router = useRouter();

    // 检查是否已经登录
    const isLoaded = ref(false);
    onMounted(async () => {
      const data = await MyGet(BackendPath + "api/userinfomation/CheckState");
      if (data != undefined && data.status === "Good") {
        router.push("/home");
      }
      isLoaded.value = true;
    });

    const id = ref("");
    const password = ref("");

    const idError = ref("");
    const passwordError = ref("");

    const handleSubmit = async (e) => {
      let errorNumber = 0;
      if (id.value.length == 0) {
        idError.value = "Id can't be empty";
        errorNumber++;
      } else {
        idError.value = "";
      }

      if (password.value.length == 0) {
        passwordError.value = "Password can't be empty";
        errorNumber++;
      } else {
        passwordError.value = "";
      }

      if (errorNumber == 0) {
        const data = await MyPost(BackendPath + "api/login", {
          id: id.value,
          keyWord: password.value,
        });

        if (data.status == "Good") {
          id.value = "";
          password.value = "";
          router.push({ name: "Home" });
        } else {
          triggerErrorToast("Error");
        }
      }
    };

    const {
      text,
      showToast,
      isError,
      triggerErrorToast,
      triggerGoodToast,
    } = getToastHandle();

    return {
      isLoaded,
      id,
      password,
      handleSubmit,
      idError,
      passwordError,
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
.login-page {
  font-size: 40px;
  box-sizing: border-box;
  height: 100vh;
  font-family: "Ubuntu", "Ma Shan Zheng", sans-serif;
}
.animation-area {
  position: absolute;
  background: linear-gradient(to left, #8942a8, #ba382f);
  width: 100%;
  height: 100%;
  min-height: 700px;
}
.box-area {
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  overflow: hidden;
}
.box-area li {
  position: fixed;
  display: block;
  list-style: none;
  width: 25px;
  height: 25px;
  background: rgba(255, 255, 255, 0.2);
  animation: animation 20s linear infinite;
  bottom: -150px;
}
.box-area li:nth-child(1) {
  left: 86%;
  width: 80px;
  height: 80px;
  animation-delay: 0s;
}
.box-area li:nth-child(2) {
  left: 8%;
  width: 30px;
  height: 30px;
  animation-delay: 1.5s;
  animation-duration: 10s;
}
.box-area li:nth-child(3) {
  left: 70%;
  width: 100px;
  height: 100px;
  animation-delay: 5.5s;
}
.box-area li:nth-child(4) {
  left: 42%;
  width: 150px;
  height: 150px;
  animation-delay: 0s;
  animation-duration: 15s;
}
.box-area li:nth-child(5) {
  left: 65%;
  width: 40px;
  height: 40px;
  animation-delay: 0s;
}
.box-area li:nth-child(6) {
  left: 15%;
  width: 110px;
  height: 110px;
  animation-delay: 3.5s;
}
@keyframes animation {
  0% {
    transform: translateY(0) rotate(0deg);
    opacity: 1;
  }
  100% {
    transform: translateY(-100vh) rotate(360deg);
    opacity: 1;
  }
}

.navbar {
  position: fixed;
  top: 0;
  /* top: 0; */
  /* display: flex; */
  /* display: block; */
  width: 100%;
  padding: 30px 0;
  z-index: 999;
  padding: 15px 0;
  background: crimson;
  text-decoration: none;
}
.navbar .max-width {
  display: flex;
  align-items: center;
  justify-content: space-around;
}
.navbar .logo {
  transition: all 0.3s ease;
}
.navbar .logo a {
  font-size: 50px;
  color: #fff;
  font-weight: 600;
}
.navbar .logo:hover {
  transform: scale(1.2);
}

/* layout styling */
.layout {
  position: absolute;
  font-size: 40px;
  height: 100%;
  width: 100%;
  padding-top: 20vh;
}
.layout .content-box {
  background: rgba(255, 255, 255, 0.8);
  text-align: center;
  padding-top: 40px;
  border-radius: 40px;
  font-size: 40px;
}
.layout .content-box h2 {
  color: #6b6d7c;
  padding-bottom: 0.5em;
  font-size: 1em;
  text-align: center;
  font-weight: 700;
  /* line-height: 28px; */
}
.layout .content-box h2 span {
  color: crimson;
}

form {
  max-width: 100%;
  margin: 0.75em auto;
  text-align: left;
  padding: 0 1em 1em 1em;
}
label {
  display: block;
  max-width: 100%;
  /* margin: 25px 0 15px; */
  margin: 1em 0 0.8em 0;
  /* font-weight: 500; */
  font-weight: bold;
  font-size: 0.5em;
  text-transform: uppercase;
  letter-spacing: 1px;
  /* color: #4a4e6d; */
  /* color: #aaa; */
  color: #4a4e6d;
}
input {
  display: block;
  /* padding: 10px 6px; */
  padding: 0.75em 0.3em;
  width: 100%;
  border: none;
  border-bottom: 1px solid #ddd;
  color: #555;
}
button {
  background: crimson;
  border: 0;
  padding: 10px 20px;
  margin-top: 20px;
  color: white;
  border-radius: 20px;
  cursor: pointer;
  font-size: 0.5em;
}
.submit {
  text-align: center;
  transition: all 0.3s ease;
}
.submit:hover {
  transform: scale(1.05);
}

.error {
  color: #ff0062;
  margin-top: 10px;
  font-size: 0.4em;
  font-weight: bold;
}
@media (max-height: 700px) {
  .layout {
    padding-top: 150px;
  }
}

/* transition styling */
.fade-enter-from {
  opacity: 0;
  transform: translateX(100px);
}
.fade-enter-to {
  opacity: 1;
  transform: translateX(0);
}
.fade-enter-active {
  transition: all 1s ease;
}
</style>
