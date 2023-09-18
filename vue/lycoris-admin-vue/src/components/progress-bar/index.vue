<template>
  <div class="process-body" :style="{ height: props.height, borderRadius: radius, width: model.processWidth }" ref="barRef">
    <div class="schedule" :style="{ height: props.height, width: model.scheduleWidth, borderRadius: radius, backgroundColor: model.scheduleColor }">
      <div style="position: relative; height: 100%; width: 100%">
        <slot name="content">
          <div v-show="model.currentValue > 0" class="content" :class="{ 'content-top': props.position == 'top', 'content-end': props.position == 'end' }" :style="{ color: props.textColor }" v-if="props.showPercentage">
            {{ parseInt(percentage * 100) }}%
          </div>
        </slot>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref, reactive, computed, watch } from 'vue';

const barRef = ref();

const model = reactive({
  processWidth: '100%',
  currentValue: 0,
  scheduleWidth: '0px',
  scheduleColor: '#48b0f7',
  interval: void 0
});

const props = defineProps({
  height: {
    type: String,
    default: '10px'
  },
  width: {
    type: String,
    default: ''
  },
  value: {
    type: Number,
    default: 0
  },
  maxValue: {
    type: Number,
    default: 100
  },
  step: {
    type: Number,
    default: 1
  },
  duration: {
    type: Number,
    default: 0.5
  },
  play: {
    type: Boolean,
    default: true
  },
  showPercentage: {
    type: Boolean,
    default: false
  },
  position: {
    type: String,
    default: 'center'
  },
  scheduleColor: {
    type: Array,
    default: void 0
  },
  textColor: {
    type: String,
    default: '#000'
  },
  animation: {
    type: Boolean,
    default: false
  },
  delay: {
    type: Number,
    default: 0
  }
});

const percentage = computed(() => {
  return Math.round((model.currentValue / props.maxValue) * 100) / 100;
});

const radius = computed(() => {
  let height = props.height.replace('px', '');
  return `${height / 2}px`;
});

onMounted(() => {
  if (props.width) {
    model.processWidth = parseInt(props.width);
  }

  if (!model.processWidth || isNaN(model.processWidth)) {
    model.processWidth = barRef.value.clientWidth;
  }

  if (props.animation) {
    watch(
      () => props.play,
      value => {
        if (value) {
          if (props.delay && props.delay > 0) {
            setTimeout(() => {
              playProcess();
            }, props.delay * 1000);
          } else {
            playProcess();
          }
        } else {
          if (props.delay && props.delay > 0) {
            setTimeout(() => {
              rollbackProcess();
            }, props.delay * 1000);
          } else {
            rollbackProcess();
          }
        }
      }
    );
  } else {
    model.scheduleWidth = `${(model.processWidth * (Math.round((props.value / props.maxValue) * 100) / 100)).toFixed(1)}px`;
  }
});

const playProcess = () => {
  if (model.interval) {
    clearInterval(model.interval);
  }

  model.interval = setInterval(() => {
    let color = calcScheduleColor(model.currentValue);

    if (color != model.scheduleColor) {
      model.scheduleColor = color;
    }

    if (model.currentValue == props.value) {
      clearInterval(model.interval);
      model.interval = void 0;
      model.scheduleWidth = `${percentage.value * 100}%`;
      return;
    }

    model.currentValue = model.currentValue + (props.step || 1);

    if (model.currentValue > props.value) {
      model.currentValue = props.value;
    }

    model.scheduleWidth = `${(model.processWidth * percentage.value).toFixed(1)}px`;
  }, (props.duration || 1) * 1000);
};

const rollbackProcess = () => {
  if (model.interval) {
    clearInterval(model.interval);
  }

  model.interval = setInterval(() => {
    if (model.currentValue <= 0) {
      model.currentValue = 0;
      clearInterval(model.interval);
      model.interval = void 0;
      model.scheduleWidth = '0px';
      return;
    }

    let color = calcScheduleColor(model.currentValue);
    if (color != model.scheduleColor) {
      model.scheduleColor = color;
    }

    model.currentValue = model.currentValue - (props.step || 1);

    if (model.currentValue <= 0) {
      model.currentValue = 0;
    }

    model.scheduleWidth = `${(model.processWidth * percentage.value).toFixed(1)}px`;
  }, (props.duration || 1) * 1000);
};

const calcScheduleColor = value => {
  if (!props.scheduleColor || !props.scheduleColor.length) {
    return '#48b0f7';
  }

  let colors = [...props.scheduleColor].sort(sortBy('value', true));

  for (let i = 0; i < colors.length; i++) {
    let item = colors[i];
    if (item.value == value) {
      return item.color;
    } else if (item.value > value) {
      if (i == 0) {
        return colors[0].color;
      } else {
        return colors[i - 1].color;
      }
    }
  }

  return colors[colors.length - 1].color;
};

function sortBy(property, asc) {
  if (asc == undefined) {
    asc = -1;
  } else {
    asc = asc ? -1 : 1;
  }
  return function (value1, value2) {
    let a = value1[property];
    let b = value2[property];
    return a < b ? asc : a > b ? asc * -1 : 0;
  };
}
</script>

<style lang="scss" scoped>
.process-body {
  position: relative;
  width: 100%;
  background-color: #fff;

  .schedule {
    position: absolute;
    left: 0;
    top: 0;
    height: 100%;
    width: 30%;
    background-color: #48b0f7;
    z-index: 2;
  }

  .content {
    padding: 0 4px;
    position: relative;
    left: 0;
    top: 0;
    height: 100%;
    width: 100%;
    z-index: 3;
    display: flex;
    justify-content: center;
    align-items: center;
    cursor: default;
    font-size: 14px;
  }

  .content-top {
    position: absolute;
    top: -30%;
    left: 50%;
    transform: translate(-50%, -100%);
    z-index: 1;
  }

  .content-end {
    justify-content: flex-end;
  }
}
</style>
