<template>
  <div class="message" v-show="onShow">
    <h1><span class="material-icons" @click="goBack"> arrow_back_ios_new </span>消息详情</h1>
    <div class="d-title"> {{ dContent }}</div>
    <div class="d-course">消息来自：{{dCid}} {{dTitle}}</div>
    <div class="d-course">发布时间：{{dDate}}</div>
    <div class="d-content">{{dContent}}</div>
   
  </div>
  <div class="message" v-show="!onShow">
    <h1>我的消息</h1>
    <div class="message-list">
      <div
        class="bar"
        v-for="message in messageList"
        :key="message"
        @click="handleClick(message)"
      >
        <div class="title">{{ message.content }}</div>
        <div class="date">{{ message.date }}</div>
      </div>
    </div>
  </div>
</template>

<script>
import { onMounted, ref } from "vue";
import { useRouter, useRoute } from "vue-router";

import MyGet from "@/composables/MyGet";
import BackendPath from "@/composables/BackendPath";

export default {
  props: {
    triggerErrorToast: { type: Function },
    triggerGoodToast: { type: Function },
  },
  setup(props) {
    //     const router = useRouter();
    //     const route = useRoute();

    const messageList = ref([]);

    const triggerErrorToast = props.triggerErrorToast;
    const triggerGoodToast = props.triggerGoodToast;

    const onShow = ref(false);
    const dTitle = ref("");
    const dCid = ref("");
    const dContent = ref("");
    const dDate = ref("");

    onMounted(async () => {
      const data = await MyGet(BackendPath + "api/NoticeCheck");
      if (data.status === "Good") {
        data.noticelist.forEach((value1, index) => {
          messageList.value.push({
            notice_id: value1.notice_id,
            course_id: value1.course_id,
            title: value1.title,
            content: value1.content,
            date: value1.date,
          });
        });
      } else {
        triggerErrorToast("get error");
      }
    });

    const handleClick = (message) => {
      onShow.value = true;
      dTitle.value = message.title;
      dCid.value = message.course_id
      dContent.value = message.content;
      dDate.value = message.date;
    };

    const goBack = ()=>{
      onShow.value=false;
    }
    return {
      messageList,
      triggerErrorToast,
      triggerGoodToast,
      handleClick,
      onShow,
      dTitle,
      dContent,
      dDate,
      dCid,
      goBack,
    };
  },
};
</script>

<style>
.message {
  width: 100%;
  padding: 40px;
}
.message h1 {
  color: white;
  padding-bottom: 20px;
  border-bottom: 3px solid #bbb;
}
.message .material-icons{
  vertical-align:auto;
  color: #fff;
  cursor: pointer;
  font-size: 28px;
  padding-right:10px ;
}
.message .message-list {
  margin-top: 20px;
}
.message .message-list .bar {
  display: inline-block;
  width: 95%;
  height: 100px;
  margin: 10px 25px;
  background: #191919;
  border-radius: 10px;
  border-top: 2px solid #c49b3b;
  cursor: pointer;
  transition: all 0.3s ease;
  color: white;
}
.message .message-list .bar:hover {
  transform: scale(1.03);
  background: #111;
}
.message .message-list .bar .title {
  margin: 30px 0 0 30px;
  font-size: 24px;
}
.message .message-list .bar .date {
  float: right;
  margin-right: 20px;
  font-size: 14px;
}
.message .d-title{
  width: 100%;
  padding-top: 30px;
  text-align: center;
  font-size: 40px;
  color: #eee;
}
.message .d-course{
  width: 100%;
  margin: 20px auto;
  text-align: center;
  font-size: 20px;
}
.message .d-content{
  font-size: 20px;
}
</style>