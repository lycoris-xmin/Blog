import CryptoJS from 'crypto-js';
import { cryptoKey } from '../config.json';

/**
 * CryptoJS 加密
 *
 * @param {String} encryptData  需要加密数据
 * @returns 加密后的数据
 * @memberof Utils
 */
const encrypt = encryptData => {
  var key = CryptoJS.enc.Utf8.parse(cryptoKey);
  var srcs = CryptoJS.enc.Utf8.parse(encryptData);
  var encrypted = CryptoJS.AES.encrypt(srcs, key, {
    mode: CryptoJS.mode.ECB,
    padding: CryptoJS.pad.Pkcs7
  });
  return encrypted.toString();
};

/**
 * CryptoJS 解密
 *
 * @param {String} encryptData  需要加密数据
 * @returns 解密后的数据
 * @memberof Utils
 */
const decrypt = encryptData => {
  var key = CryptoJS.enc.Utf8.parse(cryptoKey);
  var decrypt = CryptoJS.AES.decrypt(encryptData, key, {
    mode: CryptoJS.mode.ECB,
    padding: CryptoJS.pad.Pkcs7
  });
  return CryptoJS.enc.Utf8.stringify(decrypt).toString();
};

export default {
  encrypt,
  decrypt
};
