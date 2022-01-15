<template>
  <div class="index">
    <Navbar />
    <Home />
    <About />
    <Login />
    <Signup />
    <Team />
    <Footer />
  </div>
</template>

<script>
import Navbar from "../components/Index/Navbar";
import Home from "../components/Index/Home";
import About from "../components/Index/About";
import Signup from "../components/Index/Signup";
import Login from "../components/Index/Login";
import Team from "../components/Index/Team";
import Footer from "../components/Index/Footer";



export default {
  name: "Index",
  components: {
    Navbar,
    Home,
    About,
    Signup,
    Login,
    Team,
    Footer,
  },
  mounted() {
    (function($) {
      var typed = new Typed(".typing", {
        strings: ["考神", "学霸", "天才", "学王", "魁首", "学神"],
        typeSpeed: 100,
        backSpeed: 60,
        loop: true,
      });

      var typed = new Typed(".typing-2", {
        strings: ["考神", "学霸", "学神", "学王", "天才", "魁首"],
        typeSpeed: 100,
        backSpeed: 60,
        loop: true,
      });
      $(".carousel").owlCarousel({
        margin: 20,
        loop: true,
        autoplay: true,
        autoplayTimeOut: 200,
        autoplayHoverPause: true,
        responsive: {
          0: {
            items: 1,
            nav: false,
          },
          600: {
            items: 2,
            nav: false,
          },
          1000: {
            items: 3,
            nav: false,
          },
        },
      });
    })(jQuery);
  },
};
</script>

<style>
.about .signup,
footer {
  font-family: "Poppins", "ZCOOL KuaiLe", sans-serif;
}
.login,
.team {
  color: #fff;
  background: #111;
}
.login .title::before,

.team .title::before {
  background: #fff;
}

/* section styling */
section {
  padding: 100px 0;
}
section .title {
  position: relative;
  text-align: center;
  font-size: 40px;
  font-weight: 500;
  margin-bottom: 60px;
  padding-bottom: 20px;
  font-family: "Ubuntu", "Ma Shan Zheng", sans-serif;
}
section .title::before {
  content: "";
  position: absolute;
  bottom: 0px;
  left: 50%;
  width: 330px;
  height: 3px;
  background-color: #111;
  transform: translateX(-50%);
}
section .title::after {
  position: absolute;
  bottom: -10px;
  left: 50%;
  font-size: 20px;
  color: crimson;
  padding: 2px;
  background: #fff;
  transform: translateX(-50%);
}

/* owl styling */
.owl-dots {
  text-align: center;
  margin-top: 20px;
}
.owl-dot {
  height: 13px;
  width: 13px;
  margin: 0 5px;
  outline: nonoe !important;
  border-radius: 50%;
  border: 2px solid crimson !important;
  transition: all 0.3s ease;
}
.owl-dot.active {
  width: 35px;
  border-radius: 14px;
}
.owl-dot:hover {
  background: crimson !important;
}
</style>
