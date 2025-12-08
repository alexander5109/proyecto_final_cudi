using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace GestorDeTecnicatura {
	public static class FormMapper {



		public static void ApplyFilters<T>(
			ICollectionView view,
			DependencyObject root,
			string filterPrefix
		) {
			view.Filter = instance => {
				if (instance is T model) {
					foreach (TextBox control in GetLogicalChildren<TextBox>(root)) {
						if (string.IsNullOrWhiteSpace(control.Name) || !control.Name.StartsWith(filterPrefix))
							continue;

						string rawSuffix = control.Name.Substring(filterPrefix.Length);  // e.g., "Carrera_Displayear"
						string propPath = rawSuffix.Replace("_", ".");                   // → "Carrera.Displayear"
						string filterText = control.Text;

						if (string.IsNullOrWhiteSpace(filterText))
							continue;

						// Resolve nested property (e.g., Carrera.Displayear)
						object? value = model;
						foreach (var segment in propPath.Split('.')) {
							if (value == null)
								break;

                            PropertyInfo? prop = value.GetType().GetProperty(segment);
							if (prop == null) {
								value = null;
								break;
							}
							value = prop.GetValue(value);
						}

						if (value == null)
							return false;

						string valueStr = value.ToString() ?? string.Empty;

						if (!valueStr.Contains(filterText, StringComparison.OrdinalIgnoreCase))
							return false;
					}
					return true;
				}
				return false;
			};

			view.Refresh();
		}









		public static IEnumerable<T> GetLogicalChildren<T>(DependencyObject parent) where T : DependencyObject {
			foreach (var child in LogicalTreeHelper.GetChildren(parent)) {
				if (child is T typedChild)
					yield return typedChild;

				if (child is DependencyObject depChild) {
					foreach (T descendant in GetLogicalChildren<T>(depChild))
						yield return descendant;
				}
			}
		}
		private static object ConvertTo(string input, Type targetType) {
			try {
				if (string.IsNullOrWhiteSpace(input))
					return null;

                // Handle nullable types
                Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

				if (underlyingType.IsEnum)
					return Enum.Parse(underlyingType, input);

				return Convert.ChangeType(input, underlyingType);
			} catch {
				return null;
			}
		}


		public static void ModelToForm<T>(T model, DependencyObject root, string suffix) {
			foreach (FrameworkElement control in GetLogicalChildren<FrameworkElement>(root)) {
				if (string.IsNullOrEmpty(control.Name) || !control.Name.StartsWith(suffix))
					continue;

				string propName = control.Name.Substring(suffix.Length);
                PropertyInfo? prop = typeof(T).GetProperty(propName);
				if (prop == null)
					continue;

				var value = prop.GetValue(model);
				// MessageBox.Show(propName);
				// MessageBox.Show(value.ToString());
				switch (control) {
					case TextBox widget:
						widget.Text = value?.ToString();
						break;

					case TextBlock widget:
						widget.Text = value?.ToString();
						break;

					case DatePicker widget:
						if (value == null)
							break; // Don't touch the control if the value is null

						if (value is DateTime dt)
							widget.SelectedDate = dt;
						else if (DateTime.TryParse(value.ToString(), out DateTime parsed))
							widget.SelectedDate = parsed;
						// else do nothing
						break;

					case ItemsControl widget when widget is ListView || widget is DataGrid || widget is ItemsControl:
						// MessageBox.Show(value.ToString());
						if (value is System.Collections.IEnumerable enumerable && !(value is string))
							widget.ItemsSource = enumerable;
						break;




				}
			}
		}


		public static void FormToModel<T>(T model, DependencyObject root, string suffix) {
			foreach (FrameworkElement control in GetLogicalChildren<FrameworkElement>(root)) {
				if (string.IsNullOrEmpty(control.Name) || !control.Name.StartsWith(suffix))
					continue;

				string propName = control.Name.Substring(suffix.Length);
                PropertyInfo? prop = typeof(T).GetProperty(propName);
				if (prop == null || !prop.CanWrite)
					continue;

				object value = null;

				switch (control) {
					case TextBox textBox:
						value = ConvertTo(textBox.Text, prop.PropertyType);
						break;

					case TextBlock textBlock:
						value = ConvertTo(textBlock.Text, prop.PropertyType);
						break;

					case DatePicker datePicker:
						if (datePicker.SelectedDate != null)
							value = datePicker.SelectedDate;
						break;

					case ComboBox comboBox:
						value = comboBox.SelectedValue;
						break;

						// Add more control types as needed
				}

				if (value != null)
					prop.SetValue(model, value);
			}
		}



	}
}