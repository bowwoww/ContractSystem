function initAutocomplete(apiUrl, inputId, hiddenId) {
    $.getJSON(apiUrl, function (items) {
        $("#" + inputId).autocomplete({
            minLength: 0,
            source: function (request, response) {
                var term = $.ui.autocomplete.escapeRegex(request.term);
                var matcher = new RegExp(term, "i");
                response($.map(items, function (m) {
                    if (matcher.test(m.memberName) || matcher.test(m.memberID)) {
                        return {
                            label: m.memberName + " (" + m.memberID + ")",
                            value: m.memberName,
                            memberId: m.memberID
                        };
                    }
                }));
            },
            select: function (event, ui) {
                $("#" + inputId).val(ui.item.value);
                $("#" + hiddenId).val(ui.item.memberId);
                return false;
            },
            focus: function (event, ui) {
                $("#" + inputId).val(ui.item.value);
                return false;
            }
        });
        $("#" + inputId).on("blur", function () {
            var val = $(this).val();
            var selected = items.find(m => m.memberName === val);
            $("#" + hiddenId).val(selected ? selected.memberID : "");
        });
    });
}

function setupAutocomplete(memberInputId, memberHiddenId, trainerInputId, trainerHiddenId) {
    if (memberInputId && memberHiddenId) {
        initAutocomplete('/api/MembersApi/members', memberInputId, memberHiddenId);
    }
    if (trainerInputId && trainerHiddenId) {
        initAutocomplete('/api/MembersApi/trainers', trainerInputId, trainerHiddenId);
    }
}