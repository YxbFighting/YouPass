<template>
  <!-- about section start -->
  <section class="about" id="about">
    <div class="max-width">
      <h2 class="title">About our project</h2>
      <div class="about-content">
        <div class="column left">
          <img src="../../../public/img/about.jpg" alt="" />
        </div>
        <div class="column right">
          <div class="text">让你成为<span class="typing-2"></span></div>
          <p>
            Youpass是一款以大学生信息化教育为基础，以网络在线的形式为大学生学业水平情况诊断、学习指导和考试教学的一款综合
            性教学考试辅导系统。面向学生，它具有参与课程、进行考试、查阅检查考试与成绩等基础教学功能。面向教师，它具有创建管理课程
            、发布考试、批改试卷等等相关的教学功能。能够在这样的信息化普及度较高的时代，极大程度地增加学生的学习效率与教师的办公效率。
          </p>
        </div>
      </div>
    </div>
  </section>
</template>

<script>
export default {};
</script>

<style scoped>
.about .title::after {
  content: "what it's about";
}
.about .about-content {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: space-between;
}
.about .about-content .left {
  width: 45%;
}
.about .about-content .left img {
  height: 600px;
  width: 400px;
  object-fit: cover;
  border-radius: 6px;
}
.about .about-content .right {
  width: 55%;
  font-family: "Poppins", "ZCOOL KuaiLe", sans-serif;
}
.about .about-content .right .text {
  font-size: 35px;
  font-weight: 600;
  margin-bottom: 10px;
}
.about .about-content .right .text span {
  color: crimson;
}
.about .about-content .right p {
  text-align: justify;
}
</style>
