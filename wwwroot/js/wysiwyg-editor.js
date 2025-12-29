/**
 * Simple WYSIWYG Editor for contenteditable areas
 * Provides a toolbar with basic formatting options
 */
(function ($) {
    "use strict";

    // Store active editor instance
    let activeEditor = null;

    /**
     * Initialize WYSIWYG editor on contenteditable elements
     */
    function initializeWysiwygEditors() {
        $("[contenteditable='true']").each(function () {
            const $element = $(this);
            const elementId = $element.attr('id');
            
            if (!elementId) {
                console.warn('Contenteditable element without ID found, skipping WYSIWYG initialization');
                return;
            }

            // Wrap the element in a container if not already wrapped
            if (!$element.parent().hasClass('wysiwyg-container')) {
                $element.wrap('<div class="wysiwyg-container"></div>');
            }

            // Add event handlers
            $element.on('focus', function(e) {
                onFocusEditor(e.target);
            });

            $element.on('blur', function(e) {
                onBlurEditor(e.target);
            });

            $element.on('keydown', function(e) {
                onKeydownEditor(e);
            });
        });
    }

    /**
     * Create and show toolbar for the active editor
     */
    function createToolbar(editor) {
        // Remove any existing toolbar
        $('.wysiwyg-toolbar').remove();

        const toolbar = $('<div class="wysiwyg-toolbar"></div>');
        
        // Define toolbar buttons
        const buttons = [
            { cmd: 'bold', icon: 'bi-type-bold', title: 'Bold (Ctrl+B)' },
            { cmd: 'italic', icon: 'bi-type-italic', title: 'Italic (Ctrl+I)' },
            { cmd: 'underline', icon: 'bi-type-underline', title: 'Underline (Ctrl+U)' },
            { type: 'separator' },
            { cmd: 'insertUnorderedList', icon: 'bi-list-ul', title: 'Bullet List' },
            { cmd: 'insertOrderedList', icon: 'bi-list-ol', title: 'Numbered List' },
            { type: 'separator' },
            { cmd: 'formatBlock', value: 'p', icon: 'bi-paragraph', title: 'Paragraph' },
            { cmd: 'formatBlock', value: 'h3', icon: 'bi-type-h3', title: 'Heading 3' },
            { cmd: 'formatBlock', value: 'h4', icon: 'bi-type-h4', title: 'Heading 4' },
            { type: 'separator' },
            { cmd: 'removeFormat', icon: 'bi-eraser', title: 'Clear Formatting' }
        ];

        // Create buttons
        buttons.forEach(button => {
            if (button.type === 'separator') {
                toolbar.append('<span class="wysiwyg-separator"></span>');
            } else {
                const btn = $(`<button type="button" class="wysiwyg-btn" title="${button.title}">
                    <i class="bi ${button.icon}"></i>
                </button>`);
                
                btn.on('mousedown', function(e) {
                    e.preventDefault(); // Prevent losing focus from editor
                });

                btn.on('click', function(e) {
                    e.preventDefault();
                    executeCommand(button.cmd, button.value);
                });

                toolbar.append(btn);
            }
        });

        // Insert toolbar before the editor
        $(editor).parent().prepend(toolbar);
    }

    /**
     * Execute formatting command
     */
    function executeCommand(cmd, value = null) {
        document.execCommand(cmd, false, value);
        if (activeEditor) {
            $(activeEditor).trigger('input'); // Trigger input event to mark as changed
        }
    }

    /**
     * Handle editor focus
     */
    function onFocusEditor(editor) {
        activeEditor = editor;
        window.currentEditingElementId = editor.id;
        
        // Create and show toolbar
        createToolbar(editor);
        
        // Add focused class
        $(editor).addClass('wysiwyg-focused');
    }

    /**
     * Handle editor blur
     */
    function onBlurEditor(editor) {
        // Delay to allow toolbar clicks to register
        setTimeout(() => {
            // Only blur if we're not clicking on toolbar
            if (!$(document.activeElement).closest('.wysiwyg-toolbar').length) {
                $(editor).removeClass('wysiwyg-focused');
                $('.wysiwyg-toolbar').remove();
                
                if (activeEditor === editor) {
                    activeEditor = null;
                    window.currentEditingElementId = null;
                }
            }
        }, 200);
    }

    /**
     * Handle keyboard shortcuts and save
     */
    function onKeydownEditor(e) {
        // Ctrl+B for bold
        if (e.ctrlKey && e.key === 'b') {
            e.preventDefault();
            executeCommand('bold');
            return false;
        }
        
        // Ctrl+I for italic
        if (e.ctrlKey && e.key === 'i') {
            e.preventDefault();
            executeCommand('italic');
            return false;
        }
        
        // Ctrl+U for underline
        if (e.ctrlKey && e.key === 'u') {
            e.preventDefault();
            executeCommand('underline');
            return false;
        }

        // Enter to save (if Shift is not pressed)
        if (e.which === 13 && !e.shiftKey) {
            e.preventDefault();
            e.stopPropagation();
            e.stopImmediatePropagation();
            
            // Trigger the save dialog
            onSaveContentEditable(e);
            return false;
        }
    }

    /**
     * Handle save content
     */
    function onSaveContentEditable(e) {
        const editor = e.target;
        
        Swal.fire({
            icon: "question",
            title: window.jsLocalizer["SetStringResource_Dialog_Title"],
            text: window.jsLocalizer["SetStringResource_Dialog_Description"],
            showCancelButton: true,
            cancelButtonColor: "#d33"
        }).then((result) => {
            if (!result.isConfirmed) {
                return;
            }

            window.showPreloader();

            const request = {
                key: editor.id,
                value: editor.innerHTML, // Save HTML content
                editedById: "xxx"
            };

            $.ajax({
                type: "POST",
                contentType: "application/json",
                url: "/home/set-string-resource",
                data: JSON.stringify(request),

                success: function (response) {
                    window.location.reload(true);
                },

                error: function (xhr, status, error) {
                    window.hidePreloader();

                    Swal.fire({
                        icon: "error",
                        title: window.jsLocalizer["AlertMessage_Error_Title"],
                        html: window.jsLocalizer["SetStringResource_Error"] + `<br > ${error}`,
                        footer: window.jsLocalizer["AlertMessage_Error_Footer"]
                    });
                }
            });
        });
    }

    /**
     * Get HTML content from editor
     */
    window.getEditorContent = function(elementId) {
        const editor = document.getElementById(elementId);
        return editor ? editor.innerHTML : '';
    };

    /**
     * Set HTML content to editor
     */
    window.setEditorContent = function(elementId, content) {
        const editor = document.getElementById(elementId);
        if (editor) {
            editor.innerHTML = content;
        }
    };

    // Initialize on document ready
    $(document).ready(function() {
        initializeWysiwygEditors();
    });

})(window.$);
