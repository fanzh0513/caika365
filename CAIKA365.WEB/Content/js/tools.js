/*
 * 小工具合集（jquery组件）
 *
 * 2012-10-24 马超 创建
 */
(function(window, $, undefined){
/*
 * 格式校验对象
 */
$.verify = {
	is : function(type, str){
		switch(type){
			//身份证号
			case "id": return isIdNo(str);
			//手机号
			case "phone": return isPhone(str);
			//邮件
			case "mail" : return isMail(str);
			//中文姓名
			case "name": return isName(str);
			//qq
			case "qq" : return isQQ(str);
			//闰年
			case "leapYear" : return isLeapYear(str);
			//以下正则来源于网上，未验证前不能使用
			//中国邮政编码：[1-9]\d{5}(?!\d)
			//ipv4：\d+\.\d+\.\d+\.\d+
			//URL：[a-zA-z]+://[^s]*
			//国内电话号码：d{3}-d{8}|d{4}-d{7}
		}
	}
};
/*
 * 格式转化对象
 */
$.convert = {
	ip2num : function(){},
	num2ip : function(){},
	
	str2unicode : function(){},
	unicode2str : function(){},
	
	str2ascii : function(){},
	ascii2str : function(){},
	
	html : function(){}
};

/*
 * 精确身份证校验
 * 2012-10-23 马超 移植改造于彩票旧版代码
 */
var isIdNo = function( id ){
	//长度校验
	// && !/^\d{15}$/.test(id) 身份证长度去掉15为位
	if( !/^\d{17}(?:\d|x)$/i.test(id) )
		return false;
	//城市校验
	var citys = "11,12,13,14,15,21,22,23,31,32,33,34,35,36,37,41,42,43,44,45,46,50,51,52,53,54,61,62,63,64,65,71,81,82,91".split(","),
		myCity = id.substr(0, 2), len = id.length;
	if( $.inArray(myCity, citys) < 0 )
		return false;
	//生日校验
	var info = len == 18 ? [id.substr(6,4), +id.substr(10,2), +id.substr(12,2)] : ["19"+id.substr(6,2), +id.substr(8,2), +id.substr(10,2)],
		birthday = new Date(info.join("/")),
		birthStr = [birthday.getFullYear(), birthday.getMonth()+1, birthday.getDate()].join("");
	if( info.join("") !== birthStr )
		return false;
	//机器码校验
	if( len == 18 ){
		id = id.replace(/x$/i,"a");
		var iSum=0, i=17;
		for(; i>=0; i--)
			iSum += (Math.pow(2,i) % 11) * parseInt(id.charAt(17 - i),11);
		if(iSum%11!=1)
			return false;
	}
	return true;
},
/*
 * 是否是中文姓名
 */
isName = function( name ){
	return /^[\u0391-\uFFE5]{1}[\u0391-\uFFE5|·]{0,18}[\u0391-\uFFE5]{1}$/.test($.trim(name));
},
/*
 * 是否是手机号码
 */
isPhone = function( phone ){
	return /^1\d{10}$/.test($.trim(phone));
},
/*
 * 是否是电子邮件
 */
isMail = function( mail ){
	return /^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/.test(mail);
},
/*
 * 是否QQ号码
 */
isQQ = function( qq ){
	return /^[1-9][0-9]{4,}$/.test(qq);
},
/*
 * 是否是闰年
 */
isLeapYear = function( year ){
	return (0 == year % 4) && ((year % 100 != 0) || (year % 400 == 0));
};

})(window,jQuery);