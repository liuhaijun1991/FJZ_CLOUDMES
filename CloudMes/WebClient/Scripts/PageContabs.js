$(function () {
    function menuItem() {
        // 获取标识数据
        var dataUrl = $(this).attr('href'),
            dataIndex = $(this).data('index'),
            menuName = $.trim($(this).text()) + "-Test" ,//+ dataUrl.GetQueryString("ID"),
            flag = true;
        dataUrl = $.MES.getPathName() + dataUrl;
        if (dataUrl == undefined || $.trim(dataUrl).length == 0) return false;
        // 选项卡菜单已存在
        $(self.parent.document).find('.J_menuTab').each(function () {
            if ($(this).data('id') == dataUrl) {
                if (!$(this).hasClass('active')) {
                    $(this).addClass('active').siblings('.J_menuTab').removeClass('active');
                    scrollToTab(this);
                    // 显示tab对应的内容区
                    $(self.parent.document).find('.J_mainContent .J_iframe').each(function () {
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
            $(self.parent.document).find('.J_iframe').hide();
            var str = '<a href="javascript:;" class="active J_menuTab" data-id="' + dataUrl + '">' + menuName + ' <i class="fa fa-times-circle"></i></a>';
            $(self.parent.document).find('.J_menuTab').removeClass('active');
            // 添加选项卡对应的iframe
            var str1 = '<iframe class="J_iframe" name="iframe' + dataIndex + '" width="100%" height="100%" src="' + dataUrl + '" frameborder="0" data-id="' + dataUrl + '" seamless></iframe>';
            $(self.parent.document).find('.J_mainContent').find('iframe.J_iframe').hide().parents('.J_mainContent').append(str1);
            // 添加选项卡
            $(self.parent.document).find('.J_menuTabs .page-tabs-content').append(str);
            self.parent.window.scrollToTab($(self.parent.document).find('.J_menuTab.active'));
        }
        return false;
    }
    $('.J_menuItem').on('click', menuItem);
})