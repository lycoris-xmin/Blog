import $pinia from './pinia';
import authorize from './sections/authorize';
import owner from './sections/owner';
import enums from './sections/enums';
import screenLock from './sections/screen-lock';

export const pinia = $pinia;

export const stores = {
  authorize: authorize($pinia),
  owner: owner($pinia),
  enums: enums($pinia),
  screenLock: screenLock($pinia)
};
