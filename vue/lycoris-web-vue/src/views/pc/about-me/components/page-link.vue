<template>
  <div class="about-me-screen-view" :style="{ height: props.index > 4 ? '0px' : props.height }">
    <div class="view-body flex-center-center">
      <img src="@/assets/images/bg.jpg" alt="" />
      <div class="view-body flex-center-center">
        <div class="link-box">
          <div class="link-card">
            <div class="slogan">
              <span class="slogan-info">灵感</span>
              <span class="slogan-success">梦想</span>
              <span class="slogan-danger">代码</span>
              <span class="slogan-cyan">未来</span>
            </div>
            <div class="text flex-center-center">
              <p></p>
            </div>
            <div class="platform">
              <div v-if="stores.owner.github">
                <a :href="stores.owner.github" target="_blank">
                  <el-tooltip effect="dark" placement="bottom">
                    <template #content>
                      <span class="tooltip-text">大型交友平台,你懂得</span>
                    </template>
                    <img class="platform-icon" src="/icon/platform/github.png" />
                  </el-tooltip>
                </a>
              </div>

              <div v-if="stores.owner.qq">
                <el-tooltip effect="dark" placement="bottom">
                  <template #content>
                    <span class="tooltip-text">QQ：{{ stores.owner.qq }} 单击可复制</span>
                  </template>
                  <img class="platform-icon" src="/icon/platform/qq.png" @click="copy(stores.owner.qq)" />
                </el-tooltip>
              </div>

              <div v-if="stores.owner.wechat">
                <el-tooltip effect="dark" placement="bottom">
                  <template #content>
                    <span class="tooltip-text">微信号：{{ stores.owner.wechat }} 单击可复制</span>
                  </template>
                  <img class="platform-icon" src="/icon/platform/wechat.png" @click="copy(stores.owner.wechat)" />
                </el-tooltip>
              </div>

              <div v-if="stores.owner.email">
                <el-tooltip effect="dark" placement="bottom">
                  <template #content>
                    <span class="tooltip-text">邮箱：{{ stores.owner.email }} 单击可复制</span>
                  </template>
                  <img class="platform-icon" src="/icon/platform/email.png" @click="copy(stores.owner.email, true)" />
                </el-tooltip>
              </div>

              <div v-if="stores.owner.bilibili">
                <el-tooltip effect="dark" placement="bottom">
                  <template #content>
                    <span class="tooltip-text">不知道为什么要展示这个，但是不展示显得很空</span>
                  </template>
                  <a :href="stores.owner.bilibili" target="_blank">
                    <img class="platform-icon" src="/icon/platform/bilibili.png" />
                  </a>
                </el-tooltip>
              </div>

              <div v-if="stores.owner.music">
                <el-tooltip effect="dark" placement="bottom">
                  <template #content>
                    <span class="tooltip-text">网抑云，你懂得</span>
                  </template>
                  <a :href="stores.owner.music" target="_blank">
                    <img class="platform-icon" src="/icon/platform/music.png" />
                  </a>
                </el-tooltip>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { stores } from '@/stores';
import toast from '@/utils/toast';
import useClipboard from 'vue-clipboard3';

const { toClipboard } = useClipboard();

const props = defineProps({
  index: {
    type: Number,
    defalut: false
  },
  height: {
    type: String,
    defalut: '0px'
  }
});

const copy = async (content, isemail = false) => {
  try {
    await toClipboard(content);
    toast.info(`复制成功,${isemail ? '非必要请不要乱发邮件谢谢！' : '加好友请注明来意'}`);
  } catch (error) {}
};
</script>

<style lang="scss" scoped>
.about-me-screen-view {
  //
  .link-box {
    padding: 80px;
    background: -webkit-gradient(linear, left top, left bottom, from(#00ffd4), to(#00ffd4)) left top, -webkit-gradient(linear, left top, left bottom, from(#00ffd4), to(#00ffd4)) left top,
      -webkit-gradient(linear, left top, left bottom, from(#00ffd4), to(#00ffd4)) right top, -webkit-gradient(linear, left top, left bottom, from(#00ffd4), to(#00ffd4)) right top,
      -webkit-gradient(linear, left top, left bottom, from(#00ffd4), to(#00ffd4)) left bottom, -webkit-gradient(linear, left top, left bottom, from(#00ffd4), to(#00ffd4)) left bottom,
      -webkit-gradient(linear, left top, left bottom, from(#00ffd4), to(#00ffd4)) right bottom, -webkit-gradient(linear, left top, left bottom, from(#00ffd4), to(#00ffd4)) right bottom;
    background: -o-linear-gradient(#00ffd4, #00ffd4) left top, -o-linear-gradient(#00ffd4, #00ffd4) left top, -o-linear-gradient(#00ffd4, #00ffd4) right top, -o-linear-gradient(#00ffd4, #00ffd4) right top,
      -o-linear-gradient(#00ffd4, #00ffd4) left bottom, -o-linear-gradient(#00ffd4, #00ffd4) left bottom, -o-linear-gradient(#00ffd4, #00ffd4) right bottom, -o-linear-gradient(#00ffd4, #00ffd4) right bottom;
    background: linear-gradient(#00ffd4, #00ffd4) left top, linear-gradient(#00ffd4, #00ffd4) left top, linear-gradient(#00ffd4, #00ffd4) right top, linear-gradient(#00ffd4, #00ffd4) right top,
      linear-gradient(#00ffd4, #00ffd4) left bottom, linear-gradient(#00ffd4, #00ffd4) left bottom, linear-gradient(#00ffd4, #00ffd4) right bottom, linear-gradient(#00ffd4, #00ffd4) right bottom;
    background-repeat: no-repeat;
    background-size: 5px 60px, 60px 5px;

    $size: 350px;
    .link-card {
      height: $size;
      width: $size;
      display: flex;
      justify-content: flex-start;
      align-items: center;
      flex-direction: column;

      .slogan {
        display: flex;
        justify-content: center;
        align-items: center;
        gap: 25px;

        > span {
          font-size: 26px;
          letter-spacing: 2.5px;
          font-family: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif;
        }

        .slogan-info {
          color: var(--color-info);
        }

        .slogan-danger {
          color: var(--color-danger);
        }

        .slogan-success {
          color: var(--color-success);
        }
        .slogan-cyan {
          color: var(--color-cyan);
        }
      }

      .text {
        height: $size - 140px;
        margin: 15px 0;
        color: #fff;
      }

      .platform {
        display: flex;
        justify-content: center;
        align-items: center;
        background-color: rgb(255 255 255 / 60%);
        border-radius: 10px;
        padding: 5px 15px;
        gap: 10px;

        $icon-size: 50px;
        > div {
          height: $icon-size;

          > a {
            height: $icon-size;
            display: block;
          }
        }

        .platform-icon {
          height: $icon-size;
          width: $icon-size;
          transition: transform 0.4s;
          cursor: pointer;
        }

        .platform-icon:hover {
          transform: scale(1.2);
        }
      }
    }
  }
}

.tooltip-text {
  font-size: 16px;
}
</style>
