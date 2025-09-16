// Datepicker 中文語系
(function (factory) {
    if (typeof define === "function" && define.amd) {
        define(["../widgets/datepicker"], factory);
    } else {
        factory(jQuery.datepicker);
    }
}(function (datepicker) {
    datepicker.regional['zh-TW'] = {
        closeText: "關閉",
        prevText: "上月",
        nextText: "下月",
        currentText: "今天",
        monthNames: ["一月", "二月", "三月", "四月", "五月", "六月",
            "七月", "八月", "九月", "十月", "十一月", "十二月"],
        monthNamesShort: ["一月", "二月", "三月", "四月", "五月", "六月",
            "七月", "八月", "九月", "十月", "十一月", "十二月"],
        dayNames: ["星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"],
        dayNamesShort: ["日", "一", "二", "三", "四", "五", "六"],
        dayNamesMin: ["日", "一", "二", "三", "四", "五", "六"],
        weekHeader: "週",
        dateFormat: "yy-mm-dd",
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: true,
        yearSuffix: "年"
    };
    datepicker.setDefaults(datepicker.regional['zh-TW']);
}));

// Timepicker Addon 中文語系
if ($.timepicker) {
    $.timepicker.regional['zh-TW'] = {
        timeOnlyTitle: '選擇時間',
        timeText: '時間',
        hourText: '時',
        minuteText: '分',
        secondText: '秒',
        millisecText: '毫秒',
        microsecText: '微秒',
        timezoneText: '時區',
        currentText: '現在',
        closeText: '關閉',
        amNames: ['上午', 'AM', 'A'],
        pmNames: ['下午', 'PM', 'P'],
        // 其他可自訂屬性
    };
    $.timepicker.setDefaults($.timepicker.regional['zh-TW']);
}

function initDatePicker(withTime) {
    $.datepicker.setDefaults($.datepicker.regional["zh-TW"]);
    if (withTime) {
        $.timepicker.setDefaults($.timepicker.regional['zh-TW']);

        var now = new Date();
        var hour = now.getHours();
        var minute = now.getMinutes();

        $(".datePicker").datetimepicker({
            dateFormat: "yy/mm/dd",
            timeFormat: "HH:mm",
            hour: hour,
            minute: minute
        });
    }
    else {
        $(".datePicker").datepicker({
            dateFormat: "yy-mm-dd"
        });
    }
}
