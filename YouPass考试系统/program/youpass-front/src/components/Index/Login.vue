<template>
  <!-- login section start -->
  <section class="login" id="login">
    <div class="max-width">
      <h2 class="title">Log In</h2>
      <div class="login-content">
        <router-link :to="{ name: 'Login' }"> Go </router-link>
      </div>
    </div>
  </section>
</template>

<script>
export default {};
</script>

<style scoped>
.login {
  background: #111;
  background-size: cover;
}
.login .title::after {
  content: "let's go";
  background: #111;
}
.login .login-content {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: center;
  /* border: 3px solid black; */
}
.login a {
  display: block;
  width: 200px;
  background: crimson;
  color: #fff;
  font-size: 25px;
  padding: 12px 36px;
  margin: 150px;
  border-radius: 6px;
  border: 2px solid crimson;
  background: crimson;
  transition: all 0.3s ease;

  text-align: center;
}
.login a:hover {
  transform: scale(1.5);
}
</style>
