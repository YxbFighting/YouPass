<template>
  <section class="team" id="team">
    <div class="max-width">
      <h2 class="title">Our team</h2>
      <div class="carousel owl-carousel">
        <div class="card" v-for="member in teamMember" :key="member">
          <div class="box">
            <img :src="member.img" alt="" />
            <div class="text">{{ member.name }}</div>
            <p>{{ member.discription }}</p>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script>
import { ref } from "vue";
// import carousel from "vue-owl-carousel";
export default {
  components: {},
  setup() {
    const imgPath = "/img/";
    const teamMember = ref([
      {
        name: "谢思程",
        img: imgPath + "team_1.jpg",
        discription: "Anyway~~~",
      },
      {
        name: "吕虹霖",
        img: imgPath + "team_2.jpg",
        discription: "I can do all things~",
      },
      {
        name: "伊啸",
        img: imgPath + "team_3.jpg",
        discription: "我名字有十二个字不信你数.",
      },
      {
        name: "张纪鹏",
        img: imgPath + "team_4.jpg",
        discription: "zjp&zyq 愿我们的感情像钢铁一般坚硬.",
      },
      {
        name: "邓泉",
        img: imgPath + "team_5.jpg",
        discription: "在避风的港湾里，找不到昂扬的帆.",
      },
      {
        name: "周长华",
        img: imgPath + "team_6.jpg",
        discription:
          "我命由我不由天.",
      },
      {
        name: "柳淯之",
        img: imgPath + "team_7.jpg",
        discription:
          "**让我查一下我的历史.",
      },
      {
        name: "刘昕宇",
        img: imgPath + "team_8.jpg",
        discription: "我要走直线~-~-~",
      },
      {
        name: "叶栩冰",
        img: imgPath + "team_9.jpg",
        discription: "Live a good life meet slowly.",
      },
      {
        name: "高浩森",
        img: imgPath + "team_10.jpg",
        discription: "青青草原~~~",
      },
    ]);
    return { teamMember };
  },
};
</script>

<style scoped>
.team .title::after {
  content: "who we are";
  background: #111;
}
.team .carousel .card {
  background: #222;
  border-radius: 6px;
  padding: 25px 35px;
  transition: all 0.3s ease;
}
.team .carousel .card:hover {
  background: crimson;
  cursor: grab;
}
.team .carousel .card .box {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}
.team .carousel .card:hover .box {
  transform: scale(1.05);
}
.team .carousel .card .box .text {
  font-size: 20px;
  font-weight: 500;
  margin: 10px 0 7px 0;
}
.team .carousel .card img {
  height: 150px;
  width: 150px;
  object-fit: cover;
  border-radius: 50%;
  border: 5px solid crimson;
}
.team .carousel .card:hover img {
  border-color: #fff;
}
</style>
