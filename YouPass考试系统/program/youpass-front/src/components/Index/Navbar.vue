<template>
  <div class="navbar" :class="{ sticky: !isTop }">
    <div class="max-width">
      <div class="logo">
        <a href="#">You<span>Pass</span></a>
      </div>
      <ul class="menu">
        <li><a href="#home">Home</a></li>
        <li><a href="#about">About</a></li>
        <li><a href="#signup">SignUp</a></li>
        <li><a href="#login">LogIn</a></li>
        <li><a href="#team">Team</a></li>
      </ul>
    </div>
    <!-- <button @click="onClick">click me</button> -->
  </div>
</template>

<script>
import { ref } from "vue";
import ListenScroll from "../../composables/ListenScroll";

export default {
  setup() {
    const isTop = ref(true);
    const handleScroll = (event) => {
      if (window.scrollY > 20) {
        isTop.value = false;
      } else {
        isTop.value = true;
      }
      // console.log(window.scrollY);
    };
    ListenScroll(handleScroll);

    return { isTop };
  },
};
</script>

<style scoped>
.navbar {
  position: fixed;
  width: 100%;
  padding: 30px 0;
  z-index: 3;
  font-family: "Ubuntu", "Ma Shan Zheng", sans-serif;
  transition: all 0.3s ease;
  text-decoration: none;
}
.navbar.sticky {
  padding: 15px 0;
  background: crimson;
}
.navbar .max-width {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.navbar .logo {
  transition: all 0.3s ease;
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
  transform: scale(1.2);
}
.navbar .logo a:hover {
  color: crimson;
}
.navbar.sticky .logo a:hover {
  color: #fff;
}
.navbar.sticky .logo a span {
  color: #fff;
}
.navbar .menu li {
  list-style: none;
  display: inline-block;
  transition: all 0.3s ease;
}
.navbar .menu li:hover {
  transform: scale(1.2);
}
.navbar .menu li a {
  color: #fff;
  font-size: 25px;
  font-weight: 500;
  margin-left: 40px;
}
.navbar .menu li a:hover {
  color: crimson;
}
.navbar.sticky .menu li a:hover {
  color: #fff;
}
</style>
