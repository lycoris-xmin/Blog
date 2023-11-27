<template>
  <div class="copyright-message card-border-radius">
    <div class="copyright-body">
      <p class="title">版权声明</p>
      <div class="copyright-body">
        <div class="content">
          <p>
            1. 本站名称：<span class="height-light">{{ stores.webSetting.webName }}</span>
          </p>
          <p>
            2. 本站永久网址：<router-link to="/">
              <span class="height-light">{{ domain }}</span>
            </router-link>
          </p>
          <p>
            3. 本网站的原创文章部分资源内容可能来源于网络，仅供大家学习与参考，如有侵权，请联系博主邮箱：<span class="height-light">{{ stores.owner.email }}</span> 进行删除处理。
          </p>
          <p>4. 本站一切资源不代表本站立场，并不代表本站赞同其观点和对其真实性负责。</p>
          <p>5. 本站一律禁止以任何方式发布或转载任何违法的相关信息，访客发现请向博主举报。</p>
        </div>
        <div class="notice">
          <p>
            声明：代码实验室|版权所有，违者必究|如未注明，均为原创|本网站采用
            <a href="https://creativecommons.org/licenses/by-nc-sa/3.0/" target="_blank">
              <span>BY-NC-SA</span>
            </a>
            协议进行授权
          </p>
          <p>
            转载：转载请注明原文链接 - <span>{{ props.title }}</span>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { inject } from 'vue';
import { stores } from '@/stores';

const domain = inject('$domain');

const props = defineProps({
  title: {
    type: String,
    required: true
  }
});
</script>

<style lang="scss" scoped>
.copyright-message {
  position: relative;
  background: #2b2b2b;
  padding: 10px 30px;
  font-size: 14px;
  color: #fff;

  .copyright-body {
    padding: 20px 0 10px 0;
    cursor: default;
    font-size: 16px;
    font-weight: 500;

    p.title {
      text-align: center;
      letter-spacing: 5.5px;
      font-size: 20px;
      font-weight: 500;
    }

    .content {
      p {
        margin-bottom: 10px;
        letter-spacing: 2px;

        .height-light {
          color: var(--color-secondary);
          cursor: pointer;
          transition: animation 0.5s;
        }

        span.height-light:hover {
          background: linear-gradient(90deg, rgba(131, 58, 180, 1) 0%, rgba(253, 29, 29, 1) 33.3%, rgba(252, 176, 69, 1) 66.6%, rgba(131, 58, 180, 1) 100%);
          background-clip: text;
          color: transparent;
          background-size: 300% 100%;
          animation: gradient-text 4s infinite linear;
        }
      }
    }

    .notice {
      padding-top: 20px;
      p {
        span {
          color: var(--color-primary);
          transition: color 0.5s;
        }

        span:hover {
          color: var(--color-info);
        }
      }
    }
  }
}

.copyright-message::before {
  font-family: fantasy;
  content: '!';
  position: absolute;
  top: 15px;
  right: 20px;
  font-size: 18px;
  color: #ff7979;
  height: 32px;
  width: 32px;
  text-align: center;
  background: #ffffff;
  line-height: 34px;
  border-radius: 5px;
}

.copyright-message::after {
  content: '';
  border-left: 10px solid transparent;
  border-right: 10px solid transparent;
  border-bottom: 10px solid #2b2b2b;
  position: absolute;
  top: -8px;
  left: 48%;
}
</style>
