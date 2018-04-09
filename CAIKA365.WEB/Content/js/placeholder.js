/*
 * placeholder 组件
 * 2013-11-25 马超 创建（暂时不支持自定义参数，需要时再增加）
 * 使用方法：
   <span><input type="text" myholder="这是placeholder文案" /></span>
   组件修改为(不支持placeholder的浏览器)：
   <span class="holderBox"><input type="text" myholder="这是placeholder文案" /><i class="placeholder">这是placeholder文案</i></span>
   请使用样式定位覆盖到输入框上，实现类似效果，由于样式差异不好统一，这里不提供任何默认样式，一个建议的样式是：
   .holderBox{position:relative;}
   .holderBox .placeholder{position:absolute;z-index:2;left:6px;top:3px;color:#979797;}
 */
(function($){
//IE10的placeholder实现跟别的浏览器不一致，效果不好
var supportPlaceholder = !$.browser.msie && ('placeholder' in document.createElement('input'));
var makePlaceHolder = function( dom ){
	return $(dom).each(function(){
		var me = $(this), input = me[0].tagName.toLowerCase() == "input" ? me : me.find("input"), holder, box, iholder, check;
		if( input[0] ){
			holder = input.attr("myholder") || "";
			if( holder ){
				if( supportPlaceholder ){
					input[0].placeholder = holder;
					return;
				}
				//2014-04-16 马超 新增重复绑定的处理逻辑
				if( input[0].initPlaceHolder ){
					input.parent().find(".placeholder").html( holder );
					return;
				}
				//模拟placeholder
				input.after("<i class='placeholder'>"+ holder +"</i>");
				input[0].initPlaceHolder = true;
				//查找关键元素，提高效率
				box = input.parent().addClass("holderBox"), iholder = box.find(".placeholder");
				input.bind("keypress keyup",check = function(){
					iholder[ $.trim(input[0].value).length ? "hide" : "show" ]();
				});
				iholder.bind("click mousedown mouseup", function(e){
					e.stopPropagation();
					switch(e.type){
						case "mousedown":
						case "mouseup":
							input.triggerHandler(e.type);
							break;
						case "click":
							input.trigger(e.type);
							input[0].focus();
							break;
					}
				});
				//立即同步一次检查初始化状态
				check();
			}
		}
	});
};
/*
 * 对外接口
 */
$.fn.placeholder = function(){
	return makePlaceHolder( this );
};
})(jQuery);