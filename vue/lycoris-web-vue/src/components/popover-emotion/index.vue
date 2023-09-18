<template>
  <el-popover :placement="props.placement" width="365" :trigger="props.trigger" :persistent="props.persistent" ref="popoverRef" :virtual-ref="props.virtualRef" v-click-outside="onClickOutside">
    <template #reference> <slot name="reference"></slot> </template>

    <ul class="emo-paneld">
      <li v-for="item in emoImg" :key="item" @click="selectEmo(item)"><img :src="item" /></li>
    </ul>
  </el-popover>
</template>

<script setup>
import { ClickOutside as vClickOutside } from 'element-plus';
import { onBeforeMount, ref, unref } from 'vue';

const popoverRef = ref();
const emoImg = ref([]);

const props = defineProps({
  placement: {
    type: String,
    default: 'left'
  },
  trigger: {
    type: String,
    default: 'hover'
  },
  persistent: {
    type: Boolean,
    default: false
  },
  virtualRef: void 0
});

onBeforeMount(() => {
  const emoList = [
    '微笑',
    '撇嘴',
    '色',
    '发呆',
    '得意',
    '流泪',
    '害羞',
    '闭嘴',
    '睡',
    '大哭',
    '尴尬',
    '发怒',
    '调皮',
    '呲牙',
    '惊讶',
    '难过',
    '酷',
    '冷汗',
    '抓狂',
    '吐',
    '偷笑',
    '可爱',
    '白眼',
    '傲慢',
    '饥饿',
    '困',
    '惊恐',
    '流汗',
    '憨笑',
    '大兵',
    '奋斗',
    '咒骂',
    '疑问',
    '嘘',
    '晕',
    '折磨',
    '衰',
    '骷髅',
    '敲打',
    '再见',
    '擦汗',
    '抠鼻',
    '鼓掌',
    '糗大了',
    '坏笑',
    '左哼哼',
    '右哼哼',
    '哈欠',
    '鄙视',
    '委屈',
    '快哭了',
    '阴险',
    '亲亲',
    '吓',
    '可怜',
    '菜刀',
    '西瓜',
    '啤酒',
    '篮球',
    '乒乓',
    '咖啡',
    '饭',
    '猪头',
    '玫瑰',
    '凋谢',
    '示爱',
    '爱心',
    '心碎',
    '蛋糕',
    '闪电',
    '炸弹',
    '刀',
    '足球',
    '瓢虫',
    '便便',
    '月亮',
    '太阳',
    '礼物',
    '拥抱',
    '强',
    '弱',
    '握手',
    '胜利',
    '抱拳',
    '勾引',
    '拳头',
    '差劲',
    '爱你',
    'NO',
    'OK',
    '爱情',
    '飞吻',
    '跳跳',
    '发抖',
    '怄火',
    '转圈',
    '磕头',
    '回头',
    '跳绳',
    '挥手',
    '激动',
    '街舞',
    '献吻',
    '左太极',
    '右太极'
  ];

  for (let index = 0; index < emoList.length; index++) {
    emoImg.value.push(`https://res.wx.qq.com/mpres/htmledition/images/icon/emotion/${index}.gif`);
  }
});

const emit = defineEmits('click');

const selectEmo = item => {
  emit('click', item);
  popoverRef.value.doClose();
};

const close = () => {
  onClickOutside();
};

const onClickOutside = () => {
  unref(popoverRef).popperRef?.delayHide?.();
};

defineExpose({
  close
});
</script>

<style lang="scss" scoped>
.emo-paneld {
  display: grid;
  grid-template-columns: repeat(10, 25px);
  grid-gap: 8px;
  height: 250px;
  overflow-y: auto;

  li {
    list-style: none;

    img {
      height: 28px;
      width: 28px;
      cursor: pointer;
    }
  }

  img:hover {
    background-color: var(--color-secondary);
  }
}
</style>
