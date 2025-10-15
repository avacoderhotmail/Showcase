window.focusHelper = {
    focusById: function (id) {
        const el = document.getElementById(id);
        if (el) el.focus();
    },

    focusFirstInvalidInput: function () {
        const invalid = document.querySelector(".input-validation-error");
        if (invalid) invalid.focus();
    }
};
