export const getEmailHost = email => {
  //
  if (email.endsWith('@163.com') || email.endsWith('@qq.com')) {
    var arr1 = email.split('@');
    var arr2 = arr1[1].split('.');
    return `https://mail.${arr2[0]}.com`;
  } else if (email.endsWith('@outlook.com')) {
    return `https://www.outlook.com`;
  }

  return '';
};

export function openQQMail() {
  var form = document.createElement('form');
  form.method = 'POST';
  form.action = 'https://mail.qq.com';
  form.style.display = 'none';
  form.target = '_blank';

  document.body.appendChild(form);
  form.submit();
  form.remove();
}
