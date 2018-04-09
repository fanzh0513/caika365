/*
 * 令牌及缓存组件（Core组件）
 * 2013-08-09 马超 创建
 *
 * 主要用于减少多页面重复ajax请求或较多消耗资源操作的问题
 * 比如，一个页面需要不停的ajax轮询，如果用户打开了多个该页面，则服务器压力会无谓增大很多；
 * 常见的使用情景是：站内消息轮询（不采用服务器推的情况下）、彩种倒计时校正、开奖结果查询等等
 */
(function(window,$,Core,undefined){
//依赖函数引用
var LS = window.LS,
	JSON = window.JSON,
	parseJSON = Core.parseJSON, //之所以用Core上的parseJSON是因为线上很多ajax返回的不是标准的json数据
	

//主接口之一：运算令牌控制
//ikey			令牌名字
//timeSection	令牌有效期（单位 ms）
//fn			实际运算过程（如ajax或js运算），接收一个函数，用于设置计算结果的缓存
//callback		回调，参数是 fn 运算的结果或者缓存	
shareAction = function(key, timeSection, fn, callback){
	var cache = Helper.getCache(key), now = Helper.now(true), timeSp = cache.time ? now - cache.time : 0, isLock = !!cache.lock;
	
	//如果距离上次操作不足一个间隔长度，则不再运算(允许一个的定时器误差)
	//如果上次运算没有释放也不进行新运算
	//允许最大锁定时间为三倍的时间间隔，防止锁死现象发生
	if( (isLock && timeSp < 3*timeSection) || (timeSp > 0 && timeSp < timeSection - 50) ){
		Helper.act( callback, cache.data );
		
	//可以进行运算
	}else if( $.isFunction(fn) ){
		//设置令牌，更新缓存
		cache.time = now;
		cache.lock = 1;
		Helper.setCache( key, cache );
		fn(function( result ){
			//更新缓存，释放令牌
			cache.time = Helper.now(true);
			cache.data = result;
			cache.lock = 0;
			Helper.setCache( key, cache );
			//回调
			Helper.act( callback, cache.data );
		});
	}
},

Helper = {
	mainKey : "shareAction",
	getCache : function( key ){
		var ls = LS.get(this.mainKey)||"{}", json = /^\{.*\}$/.test(ls) ? parseJSON(ls) : {};
		return key ? json[key]||{} : json;
	},
	setCache : function( key, obj ){
		if( !key || obj == undefined )return;
		var cache = this.getCache();
		cache[key] = obj;
		LS.set( this.mainKey, this.stringify(cache) );
	},
	now : function( num ){
		var d = Core.serverTime();
		return num ? +d : d;
	},
	stringify : function( objData ){ return JSON.stringify( objData ); },
	act : function(callback, data){
		if( data == undefined ) return;
		$.isFunction(callback) && callback( data );
	}
},

/*
 * ajax功能包装
 * @shareKey	共享key，同key的ajax复用一套缓存
 * @section		缓存有效时间，需要大于等于ajax定时频率，单位毫秒，默认3000
 * @method		ajax方法，"GET"/"POST"  （后续会支持 "GET.JSON"/"POST.JSON"）
 */
shareAjax = function(shareKey, section, method, url, data, callback, key ){
	//处理可选参数
	if( isNaN(section) ){
		key = callback;
		callback = data;
		data = url;
		url = method;
		method = section;
		section = 3000;
	}
	var cb = callback||$.noop;
	if( $.isFunction(data) ){
		cb = data;
		data = null;
	}
	//共享运算
	shareAction(shareKey, section || 3000, function( setCacheFn ){
		Core[method.toLowerCase()](url, data, function(hasErr, txt){
			setCacheFn( hasErr ? "-"+hasErr+"-" : txt );
		}, key);
	}, function( txt ){
		var hasErr = /^\-\d+\-$/.test(txt);
		cb.call(Core, hasErr ? +(txt.replace(/\D/g, "")) : 0, hasErr ? "" : txt);
	});
};

/*
 * 扩展Core
 */
Core.shareGet = function (shareKey, section, url, data, callback, key)
{
    return shareAjax(shareKey, section, "GET", url, data, callback, key)
};
Core.sharePost = function (shareKey, section, url, data, callback, key)
{
    return shareAjax(shareKey, section, "POST", url, data, callback, key)
};
Core.shareAction = shareAction;
})(window,jQuery,Core);