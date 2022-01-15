<template>
  <section class="signup" id="signup">
    <div class="max-width">
      <h2 class="title">Sign Up</h2>
      <div class="signup-content">
        <div class="card">
          <router-link :to="{ name: 'Signup', params: { type: 0 } }" class="box"
            ><i class="fas fa-user-alt"></i>
            <div class="text">你是学生</div>
          </router-link>
        </div>

        <div class="card">
          <router-link :to="{ name: 'Signup', params: { type: 1 } }" class="box"
            ><i class="fas fa-user-graduate"></i>
            <div class="text">你是教师</div>
          </router-link>
        </div>
      </div>
    </div>
  </section>
</template>

<script>
export default {};
</script>

<style scoped>
.signup .title::after {
  background: #fff;
  content: "join us";
}
.signup .signup-content {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: center;
}
.signup .signup-content .card {
  width: calc(33% - 20px);
  background: #222;
  text-align: center;
  border-radius: 10px;
  padding: 20px 25px;
  cursor: pointer;
  transition: all 0.3s ease;
  margin: auto 10px;
}
.signup .signup-content .card:hover {
  background: crimson;
  transform: scale(1.05);
}
.signup .signup-content .card i {
  font-size: 50px;
  color: crimson;
}
.signup .signup-content .card:hover i {
  color: #fff;
}
.signup .signup-content .card .box {
  color: #fff;
  font-family: "Poppins", "ZCOOL KuaiLe", sans-serif;
  transition: all 0.3s ease;
}
.signup .signup-content .card .box:hover {
  transform: scale(1.05);
}
.signup .signup-content .card .text {
  font-size: 25px;
  font-weight: 500;
  margin: 10px 0 7px 0;
}
</style>
