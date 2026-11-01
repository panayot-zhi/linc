using Microsoft.Playwright;

namespace linc.E2ETests.PageObjects
{
    public static class FormErrors
    {
        /// <summary>
        /// Visible validation messages and the names of invalid fields, for failure messages.
        /// </summary>
        public static async Task<string> DescribeAsync(IPage page)
        {
            var result = await page.EvaluateAsync<string>(@"() => {
                const visible = e => !!(e.offsetWidth || e.offsetHeight || e.getClientRects().length);
                const messages = [...document.querySelectorAll('.validation-summary-errors li, .field-validation-error, .invalid-feedback, .text-danger')]
                    .filter(visible).map(e => e.innerText.trim()).filter(t => t);
                const fields = [...document.querySelectorAll('input, select, textarea')]
                    .filter(e => e.classList.contains('is-invalid') || e.classList.contains('input-validation-error') || (e.willValidate && !e.checkValidity()))
                    .map(e => (e.name || e.id) + (e.validationMessage ? ' (' + e.validationMessage + ')' : ''));
                return 'messages: [' + [...new Set(messages)].join(' | ') + '] invalid fields: [' + fields.join(', ') + ']';
            }");
            return result;
        }
    }
}
