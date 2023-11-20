<template>
  <div class="nav">
    <div class="header">
      <div class="left flex-start-center">
        <div class="header-logo">
          <router-link :to="'/'" :title="web.name"><el-image src="/icon/logo/logo-lycoirs.png" :title="web.name" lazy /></router-link>
        </div>
        <ul class="header-menus flex-start-center">
          <li v-for="item in navMenus" :key="item.name">
            <router-link :to="{ name: item.path }" v-if="!item.dropdown">
              <div>{{ item.name }}</div>
            </router-link>
            <el-dropdown v-else>
              <div>
                {{ item.name }}
              </div>
              <template #dropdown>
                <el-dropdown-menu class="nav-dropdown">
                  <el-dropdown-item v-for="dropdown in item.dropdownItem" :key="dropdown.name">
                    <router-link :to="{ name: dropdown.path }" v-if="item.dropdown">
                      <div>{{ dropdown.name }}</div>
                    </router-link>
                  </el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </li>
        </ul>
      </div>
      <div class="right flex-start-center">
        <div class="nav-icon">
          <el-tooltip effect="dark" content="静态文件服务器切换" placement="top">
            <el-button link>
              <el-icon class="static-source" :size="24" @click="changeStaticSource">
                <component :is="'sort'"></component>
              </el-icon>
            </el-button>
          </el-tooltip>
        </div>
        <div class="nav-icon">
          <el-icon :size="24" @click="searchPost">
            <component :is="'search'"></component>
          </el-icon>
        </div>
        <div class="user" v-if="stores.user && stores.user.state">
          <el-dropdown>
            <div class="flex-start-center">
              <el-image :src="stores.user.avatar">
                <template #error>
                  <img src="/images/404.png" />
                </template>
              </el-image>
              <span>{{ stores.user.nickName }}</span>
            </div>
            <template #dropdown>
              <el-dropdown-menu class="user-options">
                <a v-if="props.isAdmin && adminPath" :href="adminPath" target="_blank"><el-dropdown-item>管理后台</el-dropdown-item></a>
                <el-dropdown-item><router-link :to="{ name: 'user' }">个人中心</router-link></el-dropdown-item>
                <!-- <el-dropdown-item @click="userMessage">
                  <span v-if="stores.chat.totalUnreadMessage == 0">消息中心</span>
                  <el-badge :value="stores.chat.totalUnreadMessage" v-else> 消息中心 </el-badge>
                </el-dropdown-item> -->
                <el-dropdown-item @click="userLogout">退出登录</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
        <div class="login" v-else @click="userLogin">LOGIN</div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { inject, ref, watch } from 'vue';
import { navMenus } from '../../../router';
import { getAdmin, logout } from '../../../api/authentication';
import { stores } from '../../../stores';
import swal from '../../../utils/swal';
import secret from '../../../utils/secret';
import toast from '../../../utils/toast';
import { web } from '../../../config.json';
import { getStaticSource, setStaticSource } from '../../../utils/staticfile';
import { useRoute, useRouter } from 'vue-router';

const route = useRoute();
const router = useRouter();

const adminPath = ref('');

const chatSignalR = inject('$chat-signalR');

const props = defineProps({
  isAdmin: {
    type: Boolean,
    default: false
  },
  messageBardge: {
    type: Number,
    default: 0
  }
});

const emit = defineEmits(['login', 'search', 'userMessage']);

watch(
  () => props.isAdmin,
  async value => {
    if (value) {
      let res = await getAdmin();
      if (res && res.resCode == 0 && res.data) {
        adminPath.value = `${res.data.path}?key=${encodeURIComponent(secret.encrypt(stores.authorize.token))}`;
      }
    }
  }
);

const userLogin = () => {
  emit('login');
};

const searchPost = () => {
  emit('search');
};

const userLogout = async () => {
  let result = await swal.confirm('确定要退出登录吗?', '退出确认');
  if (result) {
    try {
      let res = await logout();
      if (res && res.resCode == 0) {
        stores.authorize.setUserLogoutState();
        stores.user.setLogoutState();
        toast.success('退出登录成功');
        chatSignalR.stop();

        if (route.name == 'user') {
          router.push({ name: 'home' });
        }
      }
    } catch (error) {}
  }
};

// const userMessage = () => {
//   emit('userMessage');
// };

const changeStaticSource = async () => {
  let value = getStaticSource();

  if (value == 'cdn') {
    setStaticSource('local');
    await swal.success('CDN可能存在失效，部分文件可能无法加载，可切换至本地仓库', '切换至本地仓库');
  } else {
    setStaticSource('cdn');
    await swal.success('本地仓库带宽较小，加载比较缓慢，可切换远端仓库提高加载速度', '切换至远端仓库');
  }

  location.reload();
};
</script>

<style lang="scss" scoped>
$menu-height: 100px;

.nav {
  position: relative;
  padding-top: 35px;
  height: $menu-height;
  width: var(--container-width);
  margin: 0 auto;

  .header {
    padding: 10px;
    height: 100%;
    width: 100%;
    display: flex;
    justify-content: space-between;
    align-items: center;

    .left {
      .header-logo {
        a {
          display: flex;
          justify-content: center;
          align-items: center;

          :deep(.el-image) {
            img {
              height: 40px;
              width: 60px;
              cursor: pointer;
            }
          }
        }
      }

      .header-menus {
        padding: 0 50px 0 20px;

        > li {
          list-style: none;

          a,
          .el-dropdown {
            display: block;
            text-decoration: none;
            line-height: 26px;
            cursor: pointer;

            div {
              color: #fff;
              font-size: 18px;
              padding: 2px 15px;
              border-radius: 5px;
              transform: scale(1);
              transition: all 0.3s;

              @media (max-width: 1920px) {
                padding: 2px 15px;
              }
            }
          }

          &:hover {
            a,
            .el-dropdown {
              div {
                transform: scale(1.2);
                background-color: var(--color-secondary);
                color: var(--color-dark);
              }
            }
          }
        }
      }
    }

    .right {
      position: relative;

      .nav-icon {
        padding: 0 20px;
        display: flex;
        align-items: center;

        i {
          transform: scale(1);
          transition: transform 0.3s;
          cursor: pointer;
          color: #fff;
        }

        i:hover {
          transform: scale(1.2);
        }

        .static-source {
          > svg {
            transform: rotate(90deg);
          }
        }
      }

      .login {
        cursor: pointer;
        padding: 2px 15px;
        color: #fff;
        transform: scale(1);
        transition: transform 0.3s;
      }

      .login:hover {
        transform: scale(1.2);
      }

      .user {
        position: relative;

        .flex-start-center {
          gap: 15px;

          .el-image {
            height: 35px;
            width: 35px;
            display: flex;
            justify-content: center;
            align-items: center;
            border-radius: 50%;
            cursor: pointer;

            :deep(img) {
              height: 35px;
              width: 35px;
              object-fit: cover;
            }
          }

          span {
            color: #fff;
            cursor: pointer;
            transition: all 0.3s;
          }

          &:hover {
            span {
              color: var(--color-info);
            }
          }
        }
      }
    }
  }
}

.user-options {
  @media (max-width: 1920px) {
    width: 120px;
  }

  :deep(.el-dropdown-menu__item) {
    padding: 10px 16px;
    letter-spacing: 2px;
  }
}

.nav-dropdown {
  min-width: 80px;

  :deep(.el-dropdown-menu__item) {
    justify-content: center;
    padding: 0;
    margin-top: 1px;
    margin-bottom: 3px;
    transition: all 0.4s;

    &:hover {
      background-color: var(--color-secondary);
    }

    a {
      display: block;
      padding: 10px 15px;
    }
  }
}
</style>
