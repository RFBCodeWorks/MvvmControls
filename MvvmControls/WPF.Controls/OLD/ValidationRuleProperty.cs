using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CustomControls.WPF
{
    /// <summary>
    /// Allows setting a ValidationRule as a Dependency Property <br/> This class cannot be inherited.
    /// <br/> Supports implicit conversion to/from <see cref="System.Windows.Controls.ValidationRule"/> objects
    /// </summary>
    public sealed class ValidationRuleDPO : DependencyObject
    {

        /// <summary>
        /// Create a new ValidationRule Dependency Property 
        /// </summary>
        /// <param name="validationRule"></param>
        public ValidationRuleDPO(ValidationRule validationRule)
        {
            ValidationRule = validationRule;
        }

        /// <summary>
        /// The Validation Rule to implicitly accept/return
        /// </summary>
        private ValidationRule ValidationRule { get; }

        #region < Validate Methods >

        /// <inheritdoc cref="ValidationRule.Validate(object, System.Globalization.CultureInfo)"/>
        public ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            return ValidationRule.Validate(value, cultureInfo);
        }

        /// <inheritdoc cref="ValidationRule.Validate(object, System.Globalization.CultureInfo, System.Windows.Data.BindingExpressionBase)"/>
        public ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo, System.Windows.Data.BindingExpressionBase owner)
        {
            return ValidationRule.Validate(value, cultureInfo, owner);
        }

        /// <inheritdoc cref="ValidationRule.Validate(object, System.Globalization.CultureInfo, System.Windows.Data.BindingGroup)"/>
        public ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo, System.Windows.Data.BindingGroup owner)
        {
            return ValidationRule.Validate(value, cultureInfo, owner);
        }


        /// <inheritdoc cref="Object.ToString"/>        
        public override string ToString()
        {
            return ValidationRule.ToString();

        }

        /// <inheritdoc cref="ValidationRule.ValidationStep"/>
        public ValidationStep ValidationStep
        {
            get => ValidationRule.ValidationStep;
            set => ValidationRule.ValidationStep = value;
        }

        /// <inheritdoc cref="ValidationRule.ValidatesOnTargetUpdated"/>
        public bool ValidatesOnTargetUpdated
        {
            get => ValidationRule.ValidatesOnTargetUpdated;
            set => ValidationRule.ValidatesOnTargetUpdated = value;
        }

        #endregion

        /// <summary>
        /// Implicitly convert the ValidationRule
        /// </summary>
        /// <param name="rule"></param>
        public static implicit operator ValidationRuleDPO(ValidationRule rule) => new ValidationRuleDPO(rule);

        /// <summary>
        /// Implicitly retrieve the ValidationRule
        /// </summary>
        /// <param name="prop"></param>
        public static explicit operator ValidationRule(ValidationRuleDPO prop) => prop.ValidationRule;

    }
}
