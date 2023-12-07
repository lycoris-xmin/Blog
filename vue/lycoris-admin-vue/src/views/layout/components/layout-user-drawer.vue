<template>
  <div class="user-drawer">
    <el-drawer v-model="model.show" :with-header="false">
      <div class="drawer-body">
        <div class="avatar flex-center-center">
          <el-avatar :size="80" :src="stores.owner.avatar" v-if="stores.owner.avatar.length"></el-avatar>
          <el-avatar :size="80" :icon="UserFilled" v-else />
        </div>
        <div class="text-center">
          {{ stores.owner.nickName }}
        </div>
        <div class="options">
          <div class="options-item" @click="showUserProfile">
            <el-icon>
              <component :is="'user-filled'"></component>
            </el-icon>
            个人资料
          </div>

          <div class="options-item" @click="showChangePassword">
            <el-icon>
              <component :is="'lock'"></component>
            </el-icon>
            修改密码
          </div>

          <div class="options-item" @click="showLoginRecord">
            <el-icon>
              <component :is="'memo'"></component>
            </el-icon>
            登录记录
          </div>

          <div class="options-item" v-if="false" @click="screenLock">
            <el-icon>
              <component :is="'lock'"></component>
            </el-icon>
            锁定屏幕
          </div>

          <div class="options-item flex-start-center" @click="userLogout">
            <el-icon>
              <component :is="'switch-button'"></component>
            </el-icon>
            退出登录
          </div>
        </div>
      </div>
    </el-drawer>

    <user-profile-modal ref="userProfile"></user-profile-modal>
    <change-password-modal ref="changePassword"></change-password-modal>
    <login-record-modal ref="loginRecord"></login-record-modal>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { UserFilled } from '@element-plus/icons-vue';
import { logout } from '../../../api/authentication';
import UserProfileModal from '../modal/user-profile-modal.vue';
import ChangePasswordModal from '../modal/change-password-modal.vue';
import LoginRecordModal from '../modal/login-record-modal.vue';
import swal from '../../../utils/swal';
import { stores } from '../../../stores';

const route = useRoute();
const router = useRouter();
const userProfile = ref(null);
const changePassword = ref(null);
const loginRecord = ref(null);

const model = reactive({
  show: false
});

const showUserProfile = function () {
  userProfile.value.show();
};

const showChangePassword = function () {
  changePassword.value.show();
};

const showLoginRecord = function () {
  loginRecord.value.show();
};

const userLogout = async function () {
  let result = await swal.confirm('确定退出登录吗？', '退出确认');
  if (result) {
    let res = await logout();
    if (res && res.resCode == 0) {
      stores.authorize.setUserLogoutState();
      model.show = false;
      router.push({ name: 'login' });
    }
  }
};

const show = () => {
  model.show = true;
};

const screenLock = () => {
  router.push({
    name: 'screen-lock',
    query: {
      path: route.path
    }
  });

  stores.screenLock.setLossOfActivity();
};

defineExpose({
  show,
  close: () => {
    model.show = false;
  }
});
</script>

<style lang="scss" scoped>
.user-drawer {
  :deep(.el-drawer) {
    width: 400px !important;
  }

  .drawer-body {
    padding-top: 30px;

    .avatar {
      height: 100px;

      .el-avatar {
        cursor: pointer;

        :deep(.el-icon) {
          height: 80px !important;
          width: 80px !important;

          svg {
            height: 55px !important;
            width: 55px !important;
          }
        }
      }
    }

    .text-center {
      font-size: 24px;
      font-weight: 500;
      cursor: default;
    }

    .options {
      padding-top: 30px;

      .options-item {
        padding: 10px 20px;
        margin-bottom: 10px;
        font-size: 18px;
        transition: all 0.5s;
        border-radius: 15px;
        cursor: pointer;
        display: flex;
        align-items: center;

        .el-icon {
          margin-right: 10px;
        }
      }

      .options-item:last-child {
        margin-bottom: 0;
      }

      .options-item:hover {
        background-color: var(--el-menu-hover-bg-color);
        color: var(--color-primary);
      }
    }
  }
}
</style>
