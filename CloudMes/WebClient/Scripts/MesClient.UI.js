var MesClientUI = function (client) {
    this.client = client;
    MesClientUI.prototype.SetLanguage = function (Page_Name) {
        var lan = $.cookie($.MES.CK_LAN_NAME);
        var PCKN = lan + "_" + Page_Name;
        this.client.GetLanguage(lan, Page_Name, function (event) {
            if (event.Status == "Pass") {
                var data = event.Data;
                $('[set-lan]').each(function () {
                    var me = $(this);
                    var a = me.attr('set-lan').split(':');
                    var p = a[0];   //文字放置位置
                    var m = a[1];   //文字的标识

                    var attr = "";
                    if (m.indexOf('=') >= 0) {
                        var s = m.split('=');
                        attr = s[0];
                        m = s[1];
                    }
                    var t = data[m];
                    //文字放置位置有（html,val等，可以自己添加）

                    switch (p) {
                        case 'html':
                            me.html(t);
                            break;
                        case 'attr':
                            me.attr(attr, t);
                            break;
                        case 'val':
                        case 'value':
                            me.val(t);
                            break;
                        default:
                            me.html(t);
                    }
                });
            }
            else {
                swal("Get language fail", event.Message, "error");
            }
        });
    };
    MesClientUI.prototype.MenuRezise = function () {
        var maxheight = 0;
        var li = $("#NavBlock .navblockShow>li>a>span.glyphicon-class");
        for (var i = 0; i < li.length; i++) {
            if (maxheight <= li.eq(i).height()) {
                maxheight = li.eq(i).height();
            }
        }
        $("#NavBlock .navblockShow>li").css('height', parseInt(maxheight + 91) + 'px');
    };
    MesClientUI.prototype.Menu = function (c) {        
        function MenuMake(n, c, d) {
            var ul = $("<ul class=\"bs-glyphicons-list " + (n == "0" ? " navblockShow" : " navblockHidden") + "\" data-navid=\"" + n + "\"></ul>");
            for (var i = 0; i < d.length; i++) {
                var li = $("<li style=\"background-color:" + d[i].STYLE_NAME + " !important\"></li>");
                var a;
                var span1 = $("<span class=\"glyphicon glyphicon-lg " + (d[i].CLASS_NAME ? d[i].CLASS_NAME : "glyphicon-cog") + "\" aria-hidden=\"true\"></span>");
                var span2 = $("<span class=\"glyphicon-class\">" + d[i].MENU_NAME + "</span>");
                if (d[i].MENU_ITEM && d[i].MENU_ITEM.length > 0) {
                    a = $("<a href=\"#\" class=\"J_menuParent\"></a>");
                    a.append(span1);
                    a.append(span2);
                    li.append(a);
                    MenuMake(d[i].ID, li, d[i].MENU_ITEM);
                }
                else {
                    a = $("<a href=\"" + (d[i].PAGE_PATH ? d[i].PAGE_PATH : "FunctionPage/404.html") + "\" class=\"J_menuItem\" " + (d[i].PAGE_PATH ? "data-index=\"" + d[i].MENU_NAME + "\"" : "") + "></a>");
                    a.append(span1);
                    a.append(span2);
                    li.append(a);
                }
                ul.append(li);
            }
            c.append(ul);
        };
        function menuItem() {
            // 获取标识数据
            var dataUrl = $(this).attr('href'),
                dataIndex = $(this).data('index'),
                menuName = $.trim($(this).text()),
                flag = true;
            if (dataUrl == undefined || $.trim(dataUrl).length == 0) return false;
            //外部页面直接新窗口打开
            if (dataUrl.startsWith('http')) {
                window.open(dataUrl, '_blank');
                return false;
            }
            // 选项卡菜单已存在
            $('.J_menuTab').each(function () {
                if ($(this).data('id') == dataUrl) {
                    if (!$(this).hasClass('active')) {
                        $(this).addClass('active').siblings('.J_menuTab').removeClass('active');
                        scrollToTab(this);
                        // 显示tab对应的内容区
                        $('.J_mainContent .J_iframe').each(function () {
                            if ($(this).data('id') == dataUrl) {
                                $(this).show().siblings('.J_iframe').hide();
                                return false;
                            }
                        });
                    }
                    flag = false;
                    return false;
                }
            });

            // 选项卡菜单不存在
            if (flag) {
                $('.J_iframe').hide();
                var str = '<a href="javascript:;" class="active J_menuTab" data-id="' + dataUrl + '">' + menuName + ' <i class="fa fa-times-circle"></i></a>';
                $('.J_menuTab').removeClass('active');
                // 添加选项卡对应的iframe
                var str1 = '<iframe class="J_iframe" name="iframe' + dataIndex + '" width="100%" height="100%" src="' + dataUrl + '" frameborder="0" data-id="' + dataUrl + '" seamless></iframe>';
                $('.J_mainContent').find('iframe.J_iframe').hide().parents('.J_mainContent').append(str1);
                // 添加选项卡
                $('.J_menuTabs .page-tabs-content').append(str);
                scrollToTab($('.J_menuTab.active'));
            }
            return false;
        };
        function menuShowItem() {
            $(this.nextElementSibling).removeClass('navblockHidden');
            $(this.nextElementSibling).addClass('bs-glyphicons-list');
            $(this.nextElementSibling).addClass('navblockShow');
            $("#NavBlock").attr('data-parent', this.parentElement.parentNode.getAttribute('data-navid'));
            $("#NavBlock").html(this.nextElementSibling.outerHTML);
            $('#NavBlock').children('.navblockHidden').remove();
            $('.J_menuItem').on('click', menuItem);
            $('.J_menuParent').on('click', menuShowItem);
            MesClientUI.prototype.MenuRezise();
        };
        function turnback() {
            var parentid = $("#NavBlock").attr('data-parent');
            var ul = $("div.hidden ul[data-navid='" + parentid + "']");
            $("#NavBlock").html(ul[0].outerHTML);
            $('#NavBlock').children().removeClass('navblockHidden');
            $('#NavBlock').children().addClass('bs-glyphicons-list');
            $('#NavBlock').children().addClass('navblockShow');
            if (parentid != "root") {
                $("#NavBlock").attr('data-parent', ul.parent().parent().attr('data-navid'));
            }
            $('#NavBlock').children('.navblockHidden').remove();
            $('.J_menuItem').on('click', menuItem);
            $('.J_menuParent').on('click', menuShowItem);
            MesClientUI.prototype.MenuRezise();
        };
        this.client.GetMenu(function (e) {
            if (e.Status == "Pass") {
                if (Storage != undefined) {
                    localStorage.MenuData = JSON.stringify(e.Data);
                }
                MenuMake("0", c, e.Data);
                var h = $("<div id=\"HiddenBlock\" class=\"hidden\"></div>");
                h.html(c[0].outerHTML);
                c.after(h);
                //通过遍历给菜单项加上data-index属性
                $(".J_menuItem").each(function (index) {
                    if (!$(this).attr('data-index')) {
                        $(this).attr('data-index', index);
                    }
                });
                $('.J_menuItem').on('click', menuItem);
                $('.J_menuParent').on('click', menuShowItem);
                MesClientUI.prototype.MenuRezise();
            }
            else {
                swal("Get Menu Fail", e.Message, "error");
            }
            $('#back').on('click', turnback);
        });
    };
    MesClientUI.prototype.MenuModify = function (c) {
        MenuMake = function (n, c, d) {
            var ul = $("<ul class=\"bs-glyphicons-list " + (n == "0" ? " navblockShow" : " navblockHidden") + " \" data-navid=\"" + n + "\"></ul>");
            for (var i = 0; i < d.length; i++) {
                var li = $("<li class=\"tf-parent\" style=\"background-color:" + d[i].STYLE_NAME + " !important\"></li>");
                var spanClose = $("<span class=\"glyphicon glyphicon-remove close-menu\" data-menuid=\"" + d[i].ID + "\"></span>");
                var spanEdit = $("<span class=\"glyphicon glyphicon-wrench edit-menu\" data-menuid=\"" + d[i].ID + "\"></span>");
                li.append(spanClose);
                li.append(spanEdit);
                var a;
                var span1 = $("<span class=\"glyphicon glyphicon-lg " + (d[i].CLASS_NAME ? d[i].CLASS_NAME : "glyphicon-cog") + "\" aria-hidden=\"true\"></span>");
                var span2 = $("<span class=\"glyphicon-class\">" + d[i].MENU_NAME + "</span>");
                a = $("<a href=\"#\" class=\"J_menuParent\"></a>");
                a.append(span1);
                a.append(span2);
                li.append(a);
                MenuMake(d[i].ID, li, d[i].MENU_ITEM);
                ul.append(li);
            }
            var NewLi = $("<li class=\"bg-10 unSortable\" data-parent-menu=\"" + n + "\">");
            var NewSpan1 = $("<span class=\"glyphicon glyphicon-lg glyphicon-plus\" aria-hidden=\"true\"></span>");
            var NewSpan2 = $("<span class=\"glyphicon-class\">New Module</span>");
            NewLi.append(NewSpan1);
            NewLi.append(NewSpan2);
            ul.append(NewLi);
            c.append(ul);
        }
        /*新建保存菜单模块方法*/
        function SaveModule() {
            var parentid = $("#MenuLi").attr("data-parent-menu");
            var Menu_Name = $("#MenuTitleShow").text();
            var IconClass = $("#MenuIconShow").attr("class");
            IconClass = IconClass.substr(IconClass.lastIndexOf(" "));
            var BGColor = $("#MenuLi").css("background-color");
            var pagePath = $("#MenuPage").val();
            var LanguageTag = $("#MenuTag").val();
            var Desc = $("#MenuDesc").val();
            var EMP_NO = self.parent.client.UserInfo.EMP_NO;
            var data = { PARENT_CODE: parentid, MENU_NAME: Menu_Name, CLASS_NAME: IconClass, STYLE_NAME: BGColor, PAGE_PATH: pagePath, MENU_DESC: Desc, LANGUAGE_ID: LanguageTag, EDIT_EMP: EMP_NO };
            self.parent.client.CallFunction("MESStation.GlobalConfig.SystemMenuConfig", "CreatMenu", data, function (e) {
                if (e.Status == "Pass") {
                    var li = $("<li class=\"tf-parent\" style=\"background-color:" + BGColor + " !important\"></li>");
                    var spanClose = $("<span class=\"glyphicon glyphicon-remove close-menu\" data-menuid=\"" + e.Data.ID + "\"></span>");
                    var spanEdit = $("<span class=\"glyphicon glyphicon-wrench edit-menu\" data-menuid=\"" + e.Data.ID + "\"></span>");
                    li.append(spanClose);
                    li.append(spanEdit);
                    var a;
                    var span1 = $("<span class=\"glyphicon glyphicon-lg " + IconClass + "\" aria-hidden=\"true\"></span>");
                    var span2 = $("<span class=\"glyphicon-class\">" + Menu_Name + "</span>");
                    a = $("<a href=\"#\" class=\"J_menuParent\"></a>");
                    a.append(span1);
                    a.append(span2);
                    li.append(a);
                    var ul = $("<ul class=\"bs-glyphicons-list navblockHidden\" data-navid=\"" + e.Data.ID + "\"></ul>");
                    var NewLi = $("<li class=\"bg-10 unSortable\" data-parent-menu=\"" + e.Data.ID + "\">");
                    var NewSpan1 = $("<span class=\"glyphicon glyphicon-lg glyphicon-plus\" aria-hidden=\"true\"></span>");
                    var NewSpan2 = $("<span class=\"glyphicon-class\">New Module</span>");
                    NewLi.append(NewSpan1);
                    NewLi.append(NewSpan2);
                    ul.append(NewLi);
                    li.append(ul);
                    var selector = "[data-navid=" + parentid + "]>.unSortable";
                    $(selector).before(li);
                    init();
                    $("#ModuleEidter").modal('hide');
                }
                else {
                    swal("Add Module Fail", e.Message, "error");
                }
            });
        }
        /*更新现有菜单模块方法*/
        function UpdateModule() {
            var parentid = $("#MenuLi").attr("data-parent-menu");
            var ID = $("#MenuLi").attr("data-navid");
            var Menu_Name = $("#MenuTitle").val();
            var IconClass = $("#MenuIconShow").attr("class");
            IconClass = IconClass.substr(IconClass.lastIndexOf(" "));
            var BGColor = $("#MenuLi").css("background-color");
            var pagePath = $("#MenuPage").val();
            var LanguageTag = $("#MenuTag").val();
            var Desc = $("#MenuDesc").val();
            var EMP_NO = self.parent.client.UserInfo.EMP_NO;
            var data = { ID: ID, PARENT_CODE: parentid, MENU_NAME: Menu_Name, CLASS_NAME: IconClass, STYLE_NAME: BGColor, PAGE_PATH: pagePath, MENU_DESC: Desc, LANGUAGE_ID: LanguageTag, EDIT_EMP: EMP_NO };
            self.parent.client.CallFunction("MESStation.GlobalConfig.SystemMenuConfig", "UpdateMenu", data, function (e) {
                if (e.Status == "Pass") {
                    $("#ModuleEidter").modal('hide');
                    swal("Save OK!", e.Message, "success");
                }
                else {
                    swal("Error", e.Message, "error");
                }
            });
        }
        /*更改排序*/
        function SortModule(e, u) {
            var thisUL = $("#NavBlock .navblockShow");
            var parentid = thisUL.attr("data-navid");
            var idArr = [];
            thisUL.children("li:not(.unSortable)").each(function () {
                idArr.push(this.firstChild.getAttribute("data-menuid"));
            });
            var selectorH = "#HiddenBlock [data-navid=" + parentid + "]";
            var selectorS = "#NavBlock [data-navid=" + parentid + "]";
            self.parent.client.CallFunction("MESStation.GlobalConfig.SystemMenuConfig", "OrderbyMenu", { PARENTID: parentid, MENUIDS: idArr }, function (e) {
                if (e.Status == "Pass") {
                    $(selectorH).html($(selectorS)[0].innerHTML);
                }
                else {
                    swal("Error", e.Message, "error", function () {
                        $(selectorS).html($(selectorH)[0].innerHTML);
                        init();
                    });
                }
            });
        }
        /*显示下级子菜单*/
        function menuShowItem() {
            $(this.nextElementSibling).removeClass('navblockHidden');
            $(this.nextElementSibling).addClass('bs-glyphicons-list');
            $(this.nextElementSibling).addClass('navblockShow');
            $("#NavBlock").attr('data-parent', this.parentElement.parentNode.getAttribute('data-navid'));
            $("#NavBlock").html(this.nextElementSibling.outerHTML);
            $('#NavBlock').children('.navblockHidden').remove();
            init();
            MesClientUI.prototype.MenuRezise();
        }
        /*菜单返回上层*/
        function turnback() {
            var parentid = $("#NavBlock").attr('data-parent');
            var ul = $("#HiddenBlock ul[data-navid='" + parentid + "']");
            $("#NavBlock").html(ul[0].outerHTML);
            $('#NavBlock').children().removeClass('navblockHidden');
            $('#NavBlock').children().addClass('bs-glyphicons-list');
            $('#NavBlock').children().addClass('navblockShow');
            if (parentid != "0") {
                $("#NavBlock").attr('data-parent', ul.parent().parent().attr('data-navid'));
            }
            $('#NavBlock').children('.navblockHidden').remove();
            init();
            MesClientUI.prototype.MenuRezise();
        }
        /*菜单模块右上角删除*/
        function menuDelete() {
            var id = this.getAttribute("data-menuid");
            swal({
                title: "Notes",
                text: "Do you want to delete the menu?",
                inputValue: id,
                showCancelButton: true,
                showConfirmButton: true
            }, function (isConfirm) {
                if (isConfirm) {
                    self.parent.client.CallFunction("MESStation.GlobalConfig.SystemMenuConfig", "DeletetMenu", { ID: this.inputValue }, function (e) {
                        if (e.Status == "Pass") {
                            var selector = "[data-menuid=" + e.Data + "]";
                            $(selector).parent().remove();
                        }
                        else {
                            swal("Error", e.Message, "error");
                        }
                    });
                }
            });
        }
        /*菜单模块右上角编辑*/
        function menuEdit() {
            var id = this.getAttribute("data-menuid");
            self.parent.client.CallFunction("MESStation.GlobalConfig.SystemMenuConfig", "GetMenuInformation", { MenuId: id }, function (e) {
                if (e.Status == "Pass") {
                    $("#MenuLi").attr("data-navid", e.Data.ID);
                    $("#MenuLi").attr("data-parent-menu");
                    $("#MenuTitleShow").text(e.Data.MENU_NAME);
                    $("#MenuTitle").val(e.Data.MENU_NAME);
                    $("#MenuIconShow").attr("class", "glyphicon glyphicon-lg " + e.Data.CLASS_NAME);
                    $("#MenuLi").css("background-color", e.Data.STYLE_NAME + " !important");
                    $("#BGColor").ClassyColor().destroy();
                    $('#BGColor').ClassyColor({
                        colorSpace: "rgba",
                        color: e.Data.STYLE_NAME,
                        staticComponents: true,
                        label: true,
                        dispalyColor: "css"
                    }).on("newcolor", function (e) {
                        $("#MenuLi").css("background-color", $('#BGColor').ClassyColor().toCssString());
                    });
                    $("#MenuPage").val(e.Data.PAGE_PATH);
                    $("#MenuTag").val(e.Data.LANGUAGE_ID);
                    $("#MenuDesc").val(e.Data.MENU_DESC);
                    $("#ModuleEidter").modal({
                        data: id,
                        Show: true
                    });
                }
                else {
                    swal("Get Menu Info", e.Message, "error");
                }
            });            
            $("#submitNewModule").off('click');
            $("#submitNewModule").on('click', UpdateModule);
        }
        /*新建菜单模块触发弹窗*/
        function NewMenuItem() {
            var id = this.getAttribute("data-parent-menu");
            $("#MenuLi").attr("data-parent-menu", id);
            $("#MenuTitleShow").text("");
            $("#MenuTitle").val("");
            $("#MenuLi").css("background-color", "#49a9e3 !important");
            $("#BGColor").ClassyColor().destroy();
            $('#BGColor').ClassyColor({
                color: "#49a9e3",
                colorSpace: "rgba",
                staticComponents: true,
                label: true,
                dispalyColor: "css"
            }).on("newcolor", function (e) {
                $("#MenuLi").css("background-color", $('#BGColor').ClassyColor().toCssString());
            });
            $("#MenuPage").val("");
            $("#MenuTag").val("");
            $("#MenuDesc").val("");
            $("#ModuleEidter").modal({
                keyboard: true,
                Show: true
            });
            $("#submitNewModule").off('click');
            $("#submitNewModule").on('click', SaveModule);
        }
        function init() {
            $('#NavBlock .J_menuParent').off('click');
            $("#NavBlock .close-menu").off('click');
            $("#NavBlock .edit-menu").off('click');
            $("#NavBlock [data-parent-menu]").off('click');

            $('#NavBlock .J_menuParent').on('click', menuShowItem);
            $("#NavBlock .close-menu").on('click', menuDelete);
            $("#NavBlock .edit-menu").on('click', menuEdit);
            $("#NavBlock [data-parent-menu]").on('click', NewMenuItem);
            /*菜单拖动排序*/
            $(".bs-glyphicons-list").sortable({
                update: SortModule,
                items: "li:not(.unSortable)"
            }).disableSelection();
        }
        this.client.GetMenu(function (e) {
            if (e.Status == "Pass") {
                MenuMake("0", c, e.Data);
                var h = $("<div id=\"HiddenBlock\" class=\"hidden\"></div>");
                h.html(c[0].innerHTML);
                c.after(h);
                init();
                MesClientUI.prototype.MenuRezise();
            }
            else {
                swal("Get Menu Fail", e.Message, "error");
            }
        });
        $('#back').on('click', turnback);
    };
    MesClientUI.prototype.QuickStart = function (o) {
        if (Storage != undefined) {
            var str = localStorage.MenuData;
            var M = JSON.parse(str);
            var arr = $.MES.getQuickStart({ Data: M, Value: o.Value, Fields: o.Fields });
            o.Container.empty();
            $("#QuickStartHid").remove();
            function MenuMake(n, c, d) {
                var ul = $("<ul class=\"bs-glyphicons-list " + (n == "0" ? " navblockShow" : " navblockHidden") + "\" data-navid=\"" + n + "\"></ul>");
                for (var i = 0; i < d.length; i++) {
                    var li = $("<li style=\"background-color:" + d[i].STYLE_NAME + " !important\"></li>");
                    var a;
                    var span1 = $("<span class=\"glyphicon glyphicon-lg " + (d[i].CLASS_NAME ? d[i].CLASS_NAME : "glyphicon-cog") + "\" aria-hidden=\"true\"></span>");
                    var span2 = $("<span class=\"glyphicon-class\">" + d[i].MENU_NAME + "</span>");
                    if (d[i].MENU_ITEM.length > 0) {
                        a = $("<a href=\"#\" class=\"J_menuParent\"></a>");
                        a.append(span1);
                        a.append(span2);
                        li.append(a);
                        MenuMake(d[i].ID, li, d[i].MENU_ITEM);
                    }
                    else {
                        a = $("<a href=\"" + (d[i].PAGE_PATH ? d[i].PAGE_PATH : "FunctionPage/404.html") + "\" class=\"J_menuItem\" " + (d[i].PAGE_PATH ? "data-index=\"" + d[i].MENU_NAME + "\"" : "") + "></a>");
                        a.append(span1);
                        a.append(span2);
                        li.append(a);
                    }
                    ul.append(li);
                }
                c.append(ul);
            };
            function menuItem() {
                // 获取标识数据
                var dataUrl = $(this).attr('href'),
                    dataIndex = $(this).data('index'),
                    menuName = $.trim($(this).text()),
                    flag = true;
                if (dataUrl == undefined || $.trim(dataUrl).length == 0) return false;

                // 选项卡菜单已存在
                $('.J_menuTab').each(function () {
                    if ($(this).data('id') == dataUrl) {
                        if (!$(this).hasClass('active')) {
                            $(this).addClass('active').siblings('.J_menuTab').removeClass('active');
                            scrollToTab(this);
                            // 显示tab对应的内容区
                            $('.J_mainContent .J_iframe').each(function () {
                                if ($(this).data('id') == dataUrl) {
                                    $(this).show().siblings('.J_iframe').hide();
                                    return false;
                                }
                            });
                        }
                        flag = false;
                        return false;
                    }
                });

                // 选项卡菜单不存在
                if (flag) {
                    $('.J_iframe').hide();
                    var str = '<a href="javascript:;" class="active J_menuTab" data-id="' + dataUrl + '">' + menuName + ' <i class="fa fa-times-circle"></i></a>';
                    $('.J_menuTab').removeClass('active');
                    // 添加选项卡对应的iframe
                    var str1 = '<iframe class="J_iframe" name="iframe' + dataIndex + '" width="100%" height="100%" src="' + dataUrl + '" frameborder="0" data-id="' + dataUrl + '" seamless></iframe>';
                    $('.J_mainContent').find('iframe.J_iframe').hide().parents('.J_mainContent').append(str1);
                    // 添加选项卡
                    $('.J_menuTabs .page-tabs-content').append(str);
                    scrollToTab($('.J_menuTab.active'));
                }
                return false;
            };
            function menuShowItem() {
                $(this.nextElementSibling).removeClass('navblockHidden');
                $(this.nextElementSibling).addClass('bs-glyphicons-list');
                $(this.nextElementSibling).addClass('navblockShow');
                $("#QuickStart").attr('data-parent', this.parentElement.parentNode.getAttribute('data-navid'));
                $("#QuickStart").html(this.nextElementSibling.outerHTML);
                $('#QuickStart').children('.navblockHidden').remove();
                init();
            };
            function turnback() {
                var parentid = $("#QuickStart").attr('data-parent');
                var ul = $("#QuickStartHid ul[data-navid='" + parentid + "']");
                $("#QuickStart").html(ul[0].outerHTML);
                $('#QuickStart').children().removeClass('navblockHidden');
                $('#QuickStart').children().addClass('bs-glyphicons-list');
                $('#QuickStart').children().addClass('navblockShow');
                if (parentid != "root") {
                    $("#QuickStart").attr('data-parent', ul.parent().parent().attr('data-navid'));
                }
                $('#QuickStart').children('.navblockHidden').remove();
                $('#QuickStart .J_menuItem').on('click', menuItem);
                $('#QuickStart .J_menuParent').on('click', menuShowItem);
            };
            function init() {
                $('#QuickStart .J_menuItem').off('click');
                $('#QuickStart .J_menuParent').off('click');
                $('#QuickStart .J_menuItem').on('click', menuItem);
                $('#QuickStart .J_menuParent').on('click', menuShowItem);
            }
            MenuMake("0", o.Container, arr);
            var h = $("<div id=\"QuickStartHid\" class=\"hidden\"></div>");
            h.html(o.Container[0].outerHTML);
            o.Container.after(h);
            //通过遍历给菜单项加上data-index属性
            $(".J_menuItem").each(function (index) {
                if (!$(this).attr('data-index')) {
                    $(this).attr('data-index', index);
                }
            });
            init();
            $('#Qback').on('click', turnback);
        }
    };
};

var MesClientUIForm = function (c) {
    this.container = c;
    MesClientUIForm.prototype.ConverSingleObj = function(obj,target) {
        if (obj.length > 0) {
            var row = obj[0];
            for (var item in row) {
                for (var targetitem in target) {
                    if (item === targetitem) {
                        target[targetitem] = row[item];
                    }
                }
            }
        }
        return target;
    };
    MesClientUIForm.prototype.SetObj = function(obj) {

    };
    MesClientUIForm.prototype.GetObj = function () {
        
    };
}

