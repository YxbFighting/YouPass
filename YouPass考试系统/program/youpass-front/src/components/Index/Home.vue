<template>
  <section class="home" id="home">
    <div class="max-width">
      <div class="home-content">
        <div class="text-1">Fighting,</div>
        <div class="text-2">
          成为
          <span class="typing"></span>
        </div>
        <a href="#signup">Join us</a>
      </div>
    </div>
  </section>
</template>

<script>
export default {};
// $(document).ready(function() {
//   (function($) {
//     // typing animation script
//     var typed = new Typed("#typing", {
//       strings: ["考神", "学霸", "天才", "学王", "魁首", "学神"],
//       typeSpeed: 100,
//       backSpeed: 60,
//       loop: true,
//     });
//   })(jQuery);
// });
</script>

<style scoped>
.home {
  display: flex;
  background: url("../../../public/img/background.jpg") no-repeat center;
  background-size: cover;
  height: 100vh;
  color: #fff;
  min-height: 500px;
  font-family: "Ubuntu", "Ma Shan Zheng", cursive, sans-serif;
}
.home .max-width {
  margin: auto 0 auto 45%;
}
.home .home-content .text-1 {
  font-size: 40px;
  margin: 5px 0;
}
.home .home-content .text-2 {
  font-size: 75px;
  margin-left: -25%;
}
.home .home-content .text-2 span {
  color: crimson;
  font-weight: 500;
}
.home .home-content a {
  display: inline-block;
  background: crimson;
  color: #fff;
  font-size: 25px;
  padding: 12px 36px;
  margin-top: 20px;
  border-radius: 6px;
  border: 2px solid crimson;
  transition: all 0.3s ease;
}
.home .home-content a:hover {
  color: crimson;
  background: none;
}
</style>
