<template>
  <ToastGroup :showToast="showToast" :isError="isError" :text="text" />
  <div class="signup-page">
    <div class="navbar">
      <div class="max-width">
        <div class="logo">
          <router-link :to="{ name: 'Index' }"
            >You<span>Pass</span></router-link
          >
          <!-- <a href="#">You<span>Pass</span></a> -->
        </div>
      </div>
    </div>
    <div class="layout">
      <div class="container">
        <div class="row">
          <div
            class="col-xl-6 offset-xl-3 col-lg-8 offset-lg-2 col-sm-10 offset-sm-1 text-center"
          >
            <transition name="content" mode="out-in" appear>
              <div class="content-box" v-if="!showResult">
                <h2>Sign up to <span>YouPass</span></h2>

                <form @submit.prevent="handleSubmit">
                  <label>Nick Name</label>
                  <input type="name" required v-model="name" />
                  <div v-if="nameError" class="error">
                    {{ nameError }}
                  </div>

                  <label>Email</label>
                  <input type="email" required v-model="email" />
                  <div v-if="emailError" class="error">
                    {{ emailError }}
                  </div>

                  <div class="block">
                    <div class="left">
                      <label>Role :</label>
                    </div>
                    <div class="right">
                      <select name="" v-model="role">
                        <option value="teacher">teacher</option>
                        <option value="student">student</option>
                      </select>
                    </div>
                  </div>

                  <label>Password</label>
                  <input type="password" required v-model="password" />
                  <div v-if="passwordError" class="error">
                    {{ passwordError }}
                  </div>

                  <label>Confirm Password</label>
                  <input type="password" required v-model="confirmPassword" />
                  <div v-if="confirmPasswordError" class="error">
                    {{ confirmPasswordError }}
                  </div>

                  <div class="submit">
                    <button>Create an account</button>
                  </div>
                </form>
              </div>

              <div v-else-if="dbright" class="content-box">
                <h2>Your Id is:</h2>
                <em>{{ id }}</em>
                <div class="go-login">
                  <router-link :to="{ name: 'Login' }">
                    Go to log in
                  </router-link>
                </div>
              </div>
              <div v-else class="content-box">
                <h2>Wrong with database</h2>
                <button @click="returnhead">
                  Go to sign up
                </button>
              </div>
            </transition>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref } from "vue";
import ToastGroup from "../components/ToastGroup";
import getToastHandle from "../composables/GetToastHandle";
import MyPost from "../composables/MyPost";
import BackendPath from "../composables/BackendPath";

export default {
  name: "Signup",
  components: { ToastGroup },
  props: { type: Number },
  setup(props) {
    const name = ref("");
    const email = ref("");
    const password = ref("");
    const confirmPassword = ref("");

    const nameError = ref("");
    const emailError = ref("");
    const passwordError = ref("");
    const confirmPasswordError = ref("");

    const showResult = ref(false);
    const dbright = ref(false);
    const id = ref("");
    const role = props.type == 1 ? ref("teacher") : ref("student");
    const loadrole = ref("");

    const returnhead = () => {
      showResult.value = false;
    };
    const handleSubmit = async () => {
      let errorNumber = 0;
      if (!(name.value.length > 0 && name.value.length <= 10)) {
        nameError.value = "Name must between 1 and 10";
        errorNumber++;
      } else {
        nameError.value = "";
      }

      if (!(password.value.length >= 5 && password.value.length <= 16)) {
        passwordError.value = "Password must between 5 and 16";
        errorNumber++;
      } else {
        passwordError.value = "";
      }

      if (
        !(
          confirmPassword.value.length >= 5 &&
          confirmPassword.value.length <= 16
        )
      ) {
        confirmPasswordError.value = "Confirm password must between 5 and 16";
        errorNumber++;
      } else if (password.value != confirmPassword.value) {
        confirmPasswordError.value = "Password must equal to confirm password";
        errorNumber++;
      } else {
        confirmPasswordError.value = "";
      }

      if (errorNumber == 0) {
        if(role.value=="teacher") loadrole.value="0"
        else loadrole.value="1"
        const data=await MyPost(BackendPath+'api/Signup',
        {
          name:name.value,
          keyWord:password.value,
          email:email.value,
          type:loadrole.value
          })
        if(data.tag=="0")
        { errorNumber++
        showResult.value = false
          emailError.value="Yout email has been signed!!"
        }
        else {
        
        emailError.value=""
        if(data.status==="Good")
        {
        
        name.value=""
        password.value=""
        confirmPassword.value=""
        email.value=""
        loadrole.value=""
        id.value=data.generatedId
        showResult.value = true
        dbright.value=true
        // triggerGoodToast("good")
        }
       
        else {
        // triggerErrorToast("Wrong")
        showResult.value = true;
        dbright.value=false
        name.value=""
        password.value=""
        confirmPassword.value=""
        email.value=""
        loadrole.value=""
        console.log(dbright)
        }}
      }
    };

    //error handling part
    const {
      text,
      showToast,
      isError,
      triggerErrorToast,
      triggerGoodToast,
    } = getToastHandle();

    return {
      name,
      email,
      role,
      password,
      id,

      //lxy add
      confirmPassword,
      dbright,
      returnhead,
      // submitsignup,
      handleSubmit,
      nameError,
      emailError,
      passwordError,
      confirmPasswordError,
      showResult,
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
.signup-page {
  height: 100vh;
  /* background: #f5f5f8; */
  font-family: "Ubuntu", "Ma Shan Zheng", sans-serif;
  background: url("../../public/img/loginBackground.jpg") no-repeat center;
  background-size: cover;
  /* display: flex; */
  /* align-items: center; */
  /* justify-content: space-around; */
  box-sizing: border-box;
  min-height: 658px;
}
.navbar {
  /* position: fixed; */
  /* top: 0; */
  /* display: flex; */
  display: block;
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

.layout {
  display: block;
  margin: 5vh auto;
  /* background: #f5f5f8; */
  font-size: 40px;
}
.layout .content-box {
  background: rgba(255, 255, 255, 0.8);
  display: block;
  text-align: center;
  /* margin: auto auto; */
  padding: 40px;
  border-radius: 40px;
  font-size: 40px;
}
.layout .content-box h2 {
  color: #6b6d7c;
  font-family: "Ubuntu", "Ma Shan Zheng", sans-serif;
  padding-bottom: 0.5em;
  font-size: 1em;
  text-align: center;
  font-weight: 700;
  /* line-height: 28px; */
}
.layout .content-box h2 span {
  color: crimson;
}
.layout .content-box em {
  font-size: 1.5em;
  color: crimson;
  display: block;
}

/* label and input styling */
form {
  max-width: 100%;
  margin: 0.75em auto;
  text-align: left;
  padding: 0 1em;
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
.role {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.role label {
  display: inline-block;
}

button,
.go-login a {
  background: crimson;
  border: 0;
  padding: 10px 20px;
  margin-top: 20px;
  color: white;
  border-radius: 20px;
  cursor: pointer;
  font-size: 0.5em;
  margin: 1em;
}
.submit,
.go-login {
  text-align: center;
  transition: all 0.3s ease;
}
.submit:hover,
.go-login:hover {
  transform: scale(1.05);
}
.go-login {
  display: block;
  padding: 10px;
}

.error {
  color: #ff0062;
  margin-top: 10px;
  font-size: 0.4em;
  font-weight: bold;
}

/* animation styling */
.content-enter-from {
  opacity: 0;
  transform: translateX(100px);
}
/* .content-enter-to {
  opacity:;
} */
.content-enter-active {
  transition: all 1s ease;
}
/* .content-leave-from {
} */
.content-leave-to {
  opacity: 0;
  transform: translateY(100px);
}
.content-leave-active {
  transition: all 1s ease;
}

@media (max-height: 845px) {
  .layout .content-box {
    font-size: 25px;
  }
}
.content-box select {
  display: inline-block;
  color: #333;
  appearance: none;
  background-color: transparent;
  border: none;
  padding: 0 1em 0 1em;
  margin: 0 0 0 0;
  font-family: inherit;
  font-size: 0.6em;
  cursor: inherit;
  line-height: inherit;
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
  outline: none;
  /* outline: none; */
}
select::-ms-expand {
  display: none;
  border: none;
}
.content-box select option {
  color: #333;
  background-color: rgb(221, 216, 216);
}
.block .left {
  display: inline-block;
  width: 20%;
}
.block .right {
  display: inline-block;
}
</style>
