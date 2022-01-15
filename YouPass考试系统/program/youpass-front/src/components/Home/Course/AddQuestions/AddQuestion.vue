<template>
  <Edit
    :defaultText="editText"
    v-if="onEditing"
    @verify="verifyEdit($event)"
    @cancel="cancelEdit"
  />
  <Answer
    :inputType="type"
    :inputOption="option"
    v-if="onShowAnswer"
    @cancel="cancelSelect"
    @verify="verifySelect($event)"
  />
  <div class="add-question">
    <div class="body-container">
      <div class="body">
        <div
          class="
            question-stage
            col-xl-8
            offset-xl-2
            col-lg-8
            offset-lg-2
            col-sm-10
            offset-sm-1
          "
        >
          <div class="content-box">
            <h2>Add Question {{ question.id }}</h2>
            <form>
              <label class="left">Type : </label>
              <select class="right" v-model="type">
                <option value="0">单项选择题</option>
                <option value="1">多项选择题</option>
                <option value="2">填空题</option>
                <option value="3">大题</option>
              </select>

              <label class="left">Subject : </label>
              <input class="input right" type="text" v-model="subject" />

              <label class="left">is Private : </label>
              <div class="checkbox-area right">
                <input class="checkbox" type="checkbox" v-model="isPrivate" />
              </div>

              <label class="left">Content : </label>
              <div class="right">
                <div class="edit" @click="onEditingContent">Edit</div>
              </div>
              <div v-html="contentHtml"></div>

              <div v-if="type === 0 || type === 1">
                <label class="left">Option : </label>
                <div class="right">
                  <div class="edit" @click="onEditing = 'Option'">+</div>
                </div>
                <div
                  class="option"
                  v-for="item in option"
                  :key="item"
                  v-html="item.contentHtml"
                  @click="onClickOption(item.id)"
                ></div>

                <label class="left">Answer : </label>
                <div class="right">
                  <div class="edit" @click="onSelectingOption">Select</div>
                </div>
                <div v-if="type === 0">
                  <div
                    v-if="standardAnswer != null"
                    v-html="
                      option.filter((value) => value.id === standardAnswer)[0]
                        .contentHtml
                    "
                  ></div>
                </div>
                <div v-else-if="standardAnswerList.length > 0">
                  <div
                    class="standard-answer"
                    v-for="id in standardAnswerList"
                    :key="id"
                    v-html="
                      option.filter((value) => value.id === id)[0].contentHtml
                    "
                  ></div>
                </div>
              </div>
              <div v-else>
                <label class="left">Answer : </label>
                <div class="right">
                  <div class="edit" @click="onEditing = 'FillingAnswer'">
                    Edit
                  </div>
                </div>
                <div v-html="fillingAnswerHtml"></div>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, getCurrentInstance, onMounted, watch } from "vue";
import _ from "lodash";

import Parse from "@/composables/LatexParse";
import Edit from "@/components/Edit.vue";
import Answer from "./Answer.vue";

export default {
  name: "AddQuestion",
  components: { Edit, Answer },
  props: {
    triggerErrorToast: { type: Function },
    triggerGoodToast: { type: Function },
    courseId: Number,
    question: {},
  },
  emits: ["questionChanged"],
  setup(props, { emit }) {
    // latex 支持
    const { ctx: _this } = getCurrentInstance();
    const reRender = () => {
      _this.$forceUpdate();
      MathJax.Hub.Typeset();
    };
    onMounted(reRender);

    // 同步消息
    const accordQuestion = () => {
      var isDone;
      if (type.value === 0) {
        isDone =
          subject.value != "" &&
          content.value != "" &&
          option.value.length > 0 &&
          standardAnswer.value != null;
      } else if (type.value === 1) {
        isDone =
          subject.value != "" &&
          content.value != "" &&
          option.value.length > 0 &&
          standardAnswerList.value.length > 0;
      } else {
        isDone =
          subject.value != "" &&
          content.value != "" &&
          fillingAnswer.value != "";
      }
      console.log(isDone);
      emit("questionChanged", {
        type: type.value,
        subject: subject.value,
        isPrivate: isPrivate.value,
        content: { content: content.value, contentHtml: contentHtml.value },
        option: option.value,
        fillingAnswer: {
          content: fillingAnswer.value,
          contentHtml: fillingAnswerHtml.value,
        },
        standardAnswer: standardAnswer.value,
        standardAnswerList: standardAnswerList.value,
        id: props.question.id,
        done: isDone,
      });
    };

    // 传给Edit组件的text
    const editText = ref("");

    // 记录题目内容
    ///////////////////////////////////////////
    // 类型
    ///////////////////////////////////////////
    const type = ref(props.question.type);
    watch(type, () => {
      type.value = Number(type.value);
      accordQuestion();
      reRender();
    });
    ///////////////////////////////////////////
    // 科目
    ///////////////////////////////////////////
    const subject = ref(props.question.subject);
    watch(subject, () => {
      accordQuestion();
    });
    ///////////////////////////////////////////
    // 是否私有
    ///////////////////////////////////////////
    const isPrivate = ref(props.question.isPrivate);
    watch(isPrivate, () => {
      accordQuestion();
    });
    ///////////////////////////////////////////
    // 题干
    ///////////////////////////////////////////
    const content = ref(props.question.content.content);
    // "公式\n$\\overbrace{1+2+\\cdots+n}^{n个} \\qquad \\underbrace{a+b+\\cdots+z}_{26}$s eius possimus in rem\
    //   soluta voluptate dolorem saepe? Aperiam, facilis. Culpa deleniti, vel\
    //   omnis impedit est dolor dignissimos provident? Atque nobis minima, "
    const contentHtml = ref(props.question.content.contentHtml);
    watch(content, () => {
      contentHtml.value = Parse(content.value);
      accordQuestion();
    });
    const onEditingContent = () => {
      onEditing.value = "Content";
      editText.value = content.value;
    };

    ///////////////////////////////////////////
    // 选择题选项
    ///////////////////////////////////////////
    const option = ref(props.question.option);
    // {
    //   content:
    //     "Lorem ipsum dolor, sit amet consectetur adipisicing elit.\
    //           Eveniet obcaecati molestiae pariatur qui possimus tempora quod\
    //           totam iste eius, corrupti fugit aperiam illum molestias rem\
    //           ullam adipisci vitae quis quibusdam velit architecto fuga quos\
    //           sed nulla nisi. Dolore laboriosam neque doloremque saepe beatae\
    //           atque consectetur nemo explicabo soluta minima mollitia, omnis\
    //           animi vitae qui? Est eaque perspiciatis ipsum? Aut, sunt neque?\
    //           Corporis enim voluptates ratione et officia, aut deserunt eaque\
    //           reiciendis consequatur tempora quae optio iure similique rem\
    //           beatae molestiae! Quidem, voluptatibus vel tempora autem nemo\
    //           sed. Eaque alias cum, quo nihil aperiam soluta est corrupti quam\
    //           qui deleniti officiis!",
    //   contentHtml: Parse(
    //     "Lorem ipsum dolor, sit amet consectetur adipisicing elit.\
    //           Eveniet obcaecati molestiae pariatur qui possimus tempora quod\
    //           totam iste eius, corrupti fugit aperiam illum molestias rem\
    //           ullam adipisci vitae quis quibusdam velit architecto fuga quos\
    //           sed nulla nisi. Dolore laboriosam neque doloremque saepe beatae\
    //           atque consectetur nemo explicabo soluta minima mollitia, omnis\
    //           animi vitae qui? Est eaque perspiciatis ipsum? Aut, sunt neque?\
    //           Corporis enim voluptates ratione et officia, aut deserunt eaque\
    //           reiciendis consequatur tempora quae optio iure similique rem\
    //           beatae molestiae! Quidem, voluptatibus vel tempora autem nemo\
    //           sed. Eaque alias cum, quo nihil aperiam soluta est corrupti quam\
    //           qui deleniti officiis!"
    //   ),
    //   id: 0,
    // },
    // {
    //   content: "234",
    //   contentHtml: "234",
    //   id: 1,
    // },
    // {
    //   content: "123",
    //   contentHtml: "123",
    //   id: 2,
    // },
    // 查看最大id函数
    const getMaxOptionId = () => {
      var id = 0;
      if (option.value.length > 0) {
        option.value.forEach((value, index) => {
          id = Math.max(id, value.id + 1);
        });
      }
      return id;
    };
    const updateId = () => {
      // 首先对option进行排序
      option.value.sort(optionCompare);
      var cnt = 0;
      option.value.forEach((value, index) => {
        option.value[index].id = cnt;
        cnt += 1;
      });
    };
    // 比较函数
    const optionCompare = (a, b) => {
      return a.id > b.id ? 1 : -1;
    };
    // 在setup的时候就进行排序
    option.value.sort(optionCompare);
    // 监听函数
    watch(
      () => [..._.cloneDeep(option.value)],
      (currentValue, oldValue) => {
        currentValue.forEach((value, index) => {
          option.value[index].contentHtml = Parse(value.content);
        });
        option.value.sort(optionCompare);
        accordQuestion();
      }
    );
    // 用来标记 当前修改是更新还是增加 -1表示增加 >0表示修改的id
    const optionAction = ref(-1);
    // 记录点击事件
    // var optionClickTimes = 0;
    var optionClickId = null;
    var optionTimer = null;
    const onClickOption = (id) => {
      if (optionClickId === id) {
        // optionClickTimes++;
        clearTimeout(optionTimer);
        // optionClickTimes = 0;
        optionClickId = null;
        onDeletingOption(id);
      } else {
        // optionClickTimes = 1;
        optionClickId = id;
        if (optionTimer != null) {
          clearTimeout(optionTimer);
        }
        optionTimer = setTimeout(() => {
          // optionClickTimes = 0;
          optionClickId = null;
          onEditingOption(id);
        }, 600);
      }
    };
    // 修改
    const onEditingOption = (id) => {
      optionAction.value = id;
      onEditing.value = "Option";
      var text = "";
      option.value.forEach((value, index) => {
        if (value.id === id) {
          text = value.content;
        }
      });
      editText.value = text;
    };
    // 删除
    const onDeletingOption = (id) => {
      option.value = option.value.filter((value) => value.id != id);
      if (type.value === 0) {
        if (standardAnswer.value === id) {
          standardAnswer.value = null;
        } else if (standardAnswer.value > id) {
          standardAnswer.value = standardAnswer.value - 1;
        }
      } else {
        standardAnswerList.value = standardAnswerList.value.filter(
          (value) => value != id
        );
        standardAnswerList.value.forEach((value, index) => {
          if (value > id) {
            standardAnswerList.value[index]--;
          }
        });
      }
      updateId();
    };
    ///////////////////////////////////////////
    // 填空题答案
    ///////////////////////////////////////////
    const fillingAnswer = ref(props.question.fillingAnswer.content);
    const fillingAnswerHtml = ref(props.question.fillingAnswer.contentHtml);
    watch(fillingAnswer, () => {
      fillingAnswerHtml.value = Parse(fillingAnswer.value);
      accordQuestion();
      setTimeout(reRender, 10);
    });
    ///////////////////////////////////////////
    // 单选题答案
    ///////////////////////////////////////////
    // 是否显示select框
    const onShowAnswer = ref(false);
    const onSelectingOption = () => {
      onShowAnswer.value = true;
    };
    const standardAnswer = ref(props.question.standardAnswer);
    watch(standardAnswer, () => {
      accordQuestion();
      setTimeout(reRender, 10);
    });
    const standardAnswerList = ref(props.question.standardAnswerList);
    watch(
      () => [...standardAnswerList.value],
      () => {
        accordQuestion();
        setTimeout(reRender, 10);
      }
    );

    // edit
    const onEditing = ref(null);
    const verifyEdit = (text) => {
      switch (onEditing.value) {
        case "Content":
          content.value = text;
          break;
        case "Option":
          // 添加+修改
          if (optionAction.value < 0) {
            const maxId = getMaxOptionId();
            option.value.push({ content: text, id: maxId });
          } else {
            option.value.forEach((value, index) => {
              if (value.id === optionAction.value) {
                option.value[index].content = text;
              }
            });
          }
          // 变为默认值 默认执行添加操作
          optionAction.value = -1;
          break;
        case "FillingAnswer":
          fillingAnswer.value = text;
          break;
        default:
          break;
      }
      editText.value = "";
      onEditing.value = null;
      setTimeout(reRender, 10);
    };
    const cancelEdit = () => {
      editText.value = "";
      onEditing.value = null;
      setTimeout(reRender, 10);
    };
    const verifySelect = (info) => {
      if (type.value === 0) {
        standardAnswer.value = info;
      } else {
        standardAnswerList.value = info;
        standardAnswerList.value.sort();
      }
      onShowAnswer.value = false;
    };
    const cancelSelect = () => {
      onShowAnswer.value = false;
    };

    return {
      type,
      subject,
      isPrivate,
      content,
      contentHtml,
      option,
      onEditing,
      verifyEdit,
      cancelEdit,
      editText,
      onClickOption,
      onEditingContent,
      fillingAnswerHtml,
      onShowAnswer,
      onSelectingOption,
      cancelSelect,
      verifySelect,
      standardAnswer,
      standardAnswerList,
    };
  },
};
</script>

<style scoped src="../../../../../public/css/Course.css"></style>

<style scoped>
@import url("https://fonts.googleapis.com/css2?family=Roboto&display=swap");
*,
*::before,
*::after {
  box-sizing: border-box;
}
div {
  word-break: break-all;
}
.add-question {
  width: 100%;
}
.question-stage {
  display: block;
  padding: 40px;
  background: #191919;
  border-radius: 10px;
  border-top: 3px solid #c49b3b;
}
.content-box h2 {
  text-align: center;
  font-size: 40px;
  color: #fff;
}
.content-box .left {
  display: inline-block;
  width: 40%;
}
.content-box .right {
  display: inline-block;
  width: 60%;
}
.content-box label {
  display: inline-block;
  color: #fff;
  font-size: 30px;
  margin: 25px 0 15px 0;
  /* margin: 1em 0 0.8em 0; */
  /* font-weight: 500; */
  font-weight: bold;
  text-transform: uppercase;
  /* letter-spacing: 1px; */
  /* color: #4a4e6d; */
  /* color: #aaa; */
}
.content-box input.input {
  display: inline-block;
  color: #ccc;
  appearance: none;
  background-color: transparent;
  border: none;
  padding: 0 1em 0 0;
  margin: 0;
  font-family: inherit;
  font-size: inherit;
  cursor: inherit;
  line-height: inherit;
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
}
.content-box input.input:focus {
  background-color: #999;
  color: #fff;
}
.content-box .checkbox-area {
  display: inline-block;
}
.content-box input.checkbox {
  display: inline-block;
  width: 25px;
  height: 25px;
  position: relative;
  margin: 0;
  z-index: 0;
}

.content-box select {
  display: inline-block;
  color: #ccc;
  appearance: none;
  background-color: transparent;
  border: none;
  padding: 0 1em 0 0;
  margin: 0;
  font-family: inherit;
  font-size: inherit;
  cursor: inherit;
  line-height: inherit;
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
  /* outline: none; */
}
select::-ms-expand {
  display: none;
}
.content-box select option {
  color: #333;
}
.content-box .modify,
.content-box .edit {
  display: inline-block;
  background: crimson;
  padding: 3px 10px;
  text-align: center;
  border-radius: 10px;
  cursor: pointer;
  font-family: "Roboto", sans-serif;
}
.content-box .edit:hover {
  color: #fff;
}
/* .content-box .modify {
  width: 40px;
  height: 40px;
  padding: auto auto;
  margin-right: 10px;
}
.content-box .modify:hover {
  color: #fff;
} */
.content-box .option {
  padding: 10px 0;
  border-radius: 10px;
  margin-bottom: 10px;
  cursor: pointer;
  transition: all 0.3s ease;
}
.content-box .option:hover {
  background: crimson;
}
.content-box .standard-answer {
  margin-bottom: 20px;
}
</style>
