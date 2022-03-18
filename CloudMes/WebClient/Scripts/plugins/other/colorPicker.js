/**
 * 调色板选择框,在Javascript中调用.
 * 
 * 调用方法:
 * 
<script language=javascript> 
	function changeColor(){
		var colorText = document.getElementById('color');
		if (colorText.value == '') colorText.value = '#FFFFFF';
		colorText.value = colorText.value.toUpperCase();
		var colorFont = document.getElementById('colorFont');
		colorFont.style.background = colorText.value;
		document.getElementById('buttonOK').focus();
	}
</script> 
<input name="color" type="text" id="color" size="10" value="#FFFFFF" alt="clrDlg" readonly />
<font id="colorFont" size="3" style="background-color:#FFFFFF;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</font>
 * 
 * 
 * Copyright 2011-11-25, Kang Lin
 * Email: kangsame@gmail.com
 */
var colorPickerHtml = function() {
    var _hex = ['FF', 'CC', '99', '66', '33', '00'];
    var builder = [];
    // 呈现一个纯色格
    var _drawPureCell = function(builder, red, green, blue) {
        builder.push('<td bgcolor="');
        if (red == '00') {
            builder.push('#FF0000');
        } else if (red == '33') {
            builder.push('#00FF00');
        } else if (red == '66') {
            builder.push('#0000FF');
        } else if (red == '99') {
            builder.push('#FFFF00');
        } else if (red == 'CC') {
            builder.push('#00FFFF');
        } else if (red == 'FF') {
            builder.push('#FF00FF');
        }
        builder.push('" unselectable="on"></td>');
    }; 
    // 呈现一个纯色格
    var _drawGrayCell = function(builder, red, green, blue) {
        builder.push('<td bgcolor="');
        if (red == '00') {
            builder.push('#000000');
        } else if (red == '33') {
            builder.push('#333333');
        } else if (red == '66') {
            builder.push('#666666');
        } else if (red == '99') {
            builder.push('#999999');
        } else if (red == 'CC') {
            builder.push('#CCCCCC');
        } else if (red == 'FF') {
            builder.push('#FFFFFF');
        }
        builder.push('" unselectable="on"></td>');
    };
    // 呈现一个颜色格
    var _drawCell = function(builder, red, green, blue) {
        builder.push('<td bgcolor="');
        builder.push('#' + red + green + blue);
        builder.push('" unselectable="on"></td>');
    };
    // 呈现一行颜色
    var _drawRow = function(builder, red, blue) {
        builder.push('<tr>');
        for (var i = 0; i < 6; ++i) {
            //呈现纯色格
            if (_hex[i] == 'FF' && blue == 'FF') {
                _drawGrayCell(builder, red, _hex[i], blue);
            } else if (_hex[i] == 'FF' && blue == '66') {
                _drawPureCell(builder, red, _hex[i], blue);
            }
            _drawCell(builder, red, _hex[i], blue);
        }
        builder.push('</tr>');
    };
    // 呈现六个颜色区之一
    var _drawTable = function(builder, blue) {
        builder.push('<table class="cell" unselectable="on">');
        for (var i = 0; i < 6; ++i) {
            _drawRow(builder, _hex[i], blue);
        }
        builder.push('</table>');
    };
    //开始创建
    builder.push('<table><tr>');
    for (var i = 0; i < 3; ++i) {
        builder.push('<td>');
        _drawTable(builder, _hex[i]);
        builder.push('</td>');
    }
    builder.push('</tr><tr>');
    for (var i = 3; i < 6; ++i) {
        builder.push('<td>');
        _drawTable(builder, _hex[i]);
        builder.push('</td>');
    }
    builder.push('</tr></table>');
    builder.push('<table class="color_result">\n' +
                  '<tr>\n' +
                    '<td class="color_view"></td>\n' +
                    '<td class="color_code"></td>\n' +
                  '</tr>\n' +
                '</table>');
    return builder.join('');
};
var addEvent = (function () {
    if (document.addEventListener) {
        return function (el, type, fn) {
            el.addEventListener(type, fn, false);
        };
    } else {
        return function (el, type, fn) {
            el.attachEvent('on' + type, function () {
                return fn.call(el, window.event);
            });
        };
    }
})();

var getTarget = function(event) {
    event = event || window.event;
    var obj = event.srcElement ? event.srcElement : event.target;
    return obj;
}
var getElementsByClassName = function (searchClass, node,tag) {
    if (document.getElementsByClassName) {
        return  document.getElementsByClassName(searchClass);
    } else {
        node = node || document;
        tag = tag || "*";
        var classes = searchClass.split(" "),
        elements = (tag === "*" && node.all)? node.all : node.getElementsByTagName(tag),
        patterns = [],
        returnElements = [],
        current,
        match;
        var i = classes.length;
        while (--i >= 0) {
            patterns.push(new RegExp("(^|\\s)" + classes[i] + "(\\s|$)"));
        }
        var j = elements.length;
        while (--j >= 0) {
            current = elements[j];
            match = false;
            for (var k=0, kl=patterns.length; k<kl; k++){
                match = patterns[k].test(current.className);
                if (!match)  break;
            }
            if (match) returnElements.push(current);
        }
        return returnElements;
    }
}
var bindClick = function(obj, id) {
    addEvent(obj, 'click', function(e) {
        var td = getTarget(e),
        nn = td.nodeName.toLowerCase(),//IE会自动转换为大写
        textfield = document.getElementById(id);
        if(nn == 'td'){
            textfield.value = td.bgColor.toUpperCase();
            obj.style.display = 'none';
            changeColor();
        }
    });
}
var bindMouseover = function(obj) {
    addEvent(obj, 'mouseover', function(e){
        var td = getTarget(e),
        nn = td.nodeName.toLowerCase(),//IE会自动转换为大写
        colorView,
        colorCode;
        if (document.querySelector) {
            colorView = obj.querySelector('td.color_view');
            colorCode = obj.querySelector('td.color_code');
        } else {
            colorView = getElementsByClassName('color_view',obj,'td')[0];
            colorCode = getElementsByClassName('color_code',obj,'td')[0];
        }
        if (nn == 'td') {
    		colorView.style.backgroundColor = td.bgColor;
    		colorCode.innerHTML = td.bgColor.toUpperCase();
        }
    });
}
var loadEvent = function(func) {
    var oldonload = window.onload;
    if (typeof window.onload != 'function') {
        window.onload = func;
    } else {
        window.onload = function() {
            oldonload();
            func();
        }
   }
}
var addSheet = function() {
    var doc, cssCode;
    if (arguments.length == 1) {
        doc = document;
        cssCode = arguments[0];
    } else if (arguments.length == 2) {
        doc = arguments[0];
        cssCode = arguments[1];
    } else {
        alert("addSheet函数最多接受两个参数!");
    }
    var headElement = doc.getElementsByTagName("head")[0];
    var styleElements = headElement.getElementsByTagName("style");
    if (styleElements.length == 0) {//如果不存在style元素则创建
        if (doc.createStyleSheet) {    //ie
            doc.createStyleSheet();
        } else {
            var tempStyleElement = doc.createElement('style');//w3c
            tempStyleElement.setAttribute("type", "text/css");
            headElement.appendChild(tempStyleElement);
        }
    }
    var  styleElement = styleElements[0];
    var media = styleElement.getAttribute("media");
    if (media != null && !/screen/.test(media.toLowerCase())) {
        styleElement.setAttribute("media","screen");
    }
    if (styleElement.styleSheet) {    //ie
        styleElement.styleSheet.cssText += cssCode;
    } else if (doc.getBoxObjectFor) {
        styleElement.innerHTML += cssCode;//火狐支持直接innerHTML添加样式表字串
    } else {
        styleElement.appendChild(doc.createTextNode(cssCode));
    }
}
var createColorPicker = function(id) {
    var textfield = document.getElementById(id);
    var picker = document.createElement('div');
    picker.setAttribute("id", "colorpicker");
    picker.innerHTML = colorPickerHtml();
    var body = document.getElementsByTagName("body")[0];
    body.insertBefore(picker, null);
    picker.style.display = 'none';
    bindMouseover(picker);
    bindClick(picker, id);
    addSheet("" +
        " div#colorpicker table{border-collapse:collapse;margin:0;padding:0;}" +
        " div#colorpicker .cell td{height:12px;width:12px;}" +
        " .color_result{width:228px;}" +
        " .color_view{width:50%;height:25px;}" +
      	" .color_code{width:50%;text-align:center;font-weight:700;color:#666;font-size:16px;background:#eee;}");
    addEvent(textfield, 'focus', function(){
        textfield.style.position = 'relative';
        picker.style.position = 'absolute';
        picker.style.display = 'block';
        picker.style.left = textfield.offsetLeft + 'px';
        picker.style.top = (textfield.clientHeight + textfield.offsetTop) + 'px';
    });
}
loadEvent(function() {
    createColorPicker("color");
});

//隐藏颜色选择框
function OnDocumentClick() {
	obj = getElement();
    var srcElement = obj;
    if (srcElement.alt == "clrDlg") {
         //显示颜色对话框
    } else {
         //是否是在颜色对话框上点击的
         while (srcElement && srcElement.id != "colorpicker") {
             srcElement = srcElement.parentElement;
         }
         if (!srcElement) {
             //不是在颜色对话框上点击的
             document.getElementById('colorpicker').style.display = 'none';
         }
    }
}
function getElement() {   //IE下的srcElement 与firefox 的兼容
	var evt = getEvent();
	var element = evt.srcElement || evt.target;
	return element;
}
function getEvent() { ////event ie and firefox 的兼容问题
	if (document.all) return window.event;//如果是ie
	func = getEvent.caller;
    while (func != null) {
        var arg0 = func.arguments[0];
        if (arg0) {if ((arg0.constructor == Event || arg0.constructor == MouseEvent) || (typeof(arg0) == "object" && arg0.preventDefault && arg0.stopPropagation)){return arg0;}}
        func = func.caller;
    }
   	return null;
}
document.onclick = function() {
	OnDocumentClick();
}
