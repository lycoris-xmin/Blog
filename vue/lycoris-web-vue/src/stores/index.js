import $pinia from './pinia';
import authorize from './sections/authorize';
import user from './sections/user';
import owner from './sections/owner';
import enums from './sections/enums';
import browse from './sections/browse';
import otherUserInfo from './sections/other-user-info';
import chat from './sections/chat';
import chatMessage from './sections/chat-message';

export const pinia = $pinia;

export const stores = {
  authorize: authorize($pinia),
  user: user($pinia),
  owner: owner($pinia),
  enums: enums($pinia),
  browse: browse($pinia),
  userInfo: otherUserInfo($pinia),
  chat: chat($pinia),
  chatMessage: chatMessage($pinia)
};
