using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AnimalLibrary;
using Microsoft.Win32;

namespace task3;

public partial class MainWindow : Window
{
    private Type? _selectedType;
    private object _createdInstance;
    
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private void button1Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "DLL Files (*.dll)|*.dll";
        if (openFileDialog.ShowDialog() == true)
        {
            string assemblyName = openFileDialog.FileName;
            var assembly = Assembly.LoadFile(assemblyName);
            Type abstractClassType = assembly.GetType("AnimalLibrary.Animal");
            Type[] implementedClasses = GetImplementedClasses(assembly.GetTypes(), abstractClassType);

            FillComboBox(implementedClasses);
        }
    }
    
    private Type[] GetImplementedClasses(Type[] types, Type subtype)
    {
        return types.Where(t => t.IsClass)
            .Where(t => t.IsSubclassOf(subtype))
            .Where(t => t.IsAbstract == false).ToArray();
    }
    
    private void FillComboBox(Type[] types)
    {
        combo0.ItemsSource = types.Select(t => t.FullName);
        combo0.Background = Brushes.LightYellow;
        combo0.Foreground = Brushes.DarkBlue;
/*        combo0.Resources.Add(SystemColors.WindowBrushKey, Brushes.Yellow);
        combo0.Resources.Add(SystemColors.HighlightBrushKey, Brushes.Red);*/
    }
    
    private void combo0_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string selectedClassName = combo0.SelectedItem as string;
        button2.Background = Brushes.LightGoldenrodYellow;
        if (!string.IsNullOrEmpty(selectedClassName))
        {
            Assembly assem = typeof(Animal).Assembly;
            _selectedType = assem.GetType(selectedClassName);
            if (_selectedType != null)
            {
                ConstructorInfo[] constructors = _selectedType.GetConstructors();
                stack1.Children.Clear();
                foreach (ConstructorInfo constructor in constructors)
                {
                    StackPanel panel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(0, 5, 0, 5)
                    };

                    ParameterInfo[] parameters = constructor.GetParameters();
                    foreach (ParameterInfo param in parameters)
                    {
                        TextBox textBox = new TextBox();
                        textBox.Margin = new Thickness(5, 40, 5, 5);
                        textBox.Tag = param.ParameterType;
                        textBox.FontSize = 22;
                        if (param.Name != null) textBox.Text = "enter " + param.Name;
                        panel.Children.Add(textBox);
                    }

                    stack1.Children.Add(panel);
                }
                button2.IsEnabled = true;
            }
        }
    }
    
    private void button2_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedType != null)
        {
            try
            {
                List<object> parameters = new List<object>();
                foreach (StackPanel panel in stack1.Children)
                {
                    foreach (TextBox textBox in panel.Children)
                    {
                        object value = Convert.ChangeType(textBox.Text, (Type)textBox.Tag);
                        parameters.Add(value);
                    }
                }
                _createdInstance = Activator.CreateInstance(_selectedType, parameters.ToArray());

                if (_createdInstance is Dog dog)
                {
                    dog.GetSpeed();
                }


                MethodInfo[] methods = _selectedType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                methods = methods.Where(m => m.GetParameters().All(p => 
                    p.ParameterType != typeof(EventHandler<>))).ToArray();

                combo1.ItemsSource = methods;
                button3.IsEnabled = true;
                button3.Background = Brushes.Yellow;
                combo1.Background = Brushes.LightYellow;
                combo1.Foreground = Brushes.DarkBlue;
/*                combo1.Resources.Add(SystemColors.WindowBrushKey, Brushes.Yellow);
                combo1.Resources.Add(SystemColors.HighlightBrushKey, Brushes.Red);*/

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating instance: " + ex.Message);
            }
        }
    }
    
    private void combo1_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        MethodInfo selectedMethod = (MethodInfo)combo1.SelectedItem;
        if (selectedMethod != null)
        {
            MethodParameterStackPanel.Children.Clear();
            ParameterInfo[] parameters = selectedMethod.GetParameters();
            foreach (ParameterInfo param in parameters)
            {
                StackPanel stackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 5, 0, 5)
                };

                Label label = new Label
                {
                    Content = $"{param.ParameterType.Name} {param.Name}:",
                    Margin = new Thickness(0, 0, 5, 0)
                };
                stackPanel.Children.Add(label);

                TextBox textBox = new TextBox
                {
                    Tag = new Tuple<Type, string>(param.ParameterType, param.Name),
                    Text = string.Empty,
                    Width = 100
                };
                stackPanel.Children.Add(textBox);

                MethodParameterStackPanel.Children.Add(stackPanel);
            }
            button3.IsEnabled = true;
        }
    }

    private void button3_Click(object sender, RoutedEventArgs e)
    {
        MethodInfo selectedMethod = (MethodInfo)combo1.SelectedItem;
        if (selectedMethod != null)
        {
            try
            {
                List<object> parameters = new List<object>();
                foreach (StackPanel stackPanel in MethodParameterStackPanel.Children)
                {
                    TextBox textBox = (TextBox)stackPanel.Children[1];
                    Tuple<Type, string> tagInfo = (Tuple<Type, string>)textBox.Tag;
                    Type parameterType = tagInfo.Item1;
                    string parameterName = tagInfo.Item2;

                    if (string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        throw new ArgumentException($"No value provided for the '{parameterName}' parameter.");
                    }

                    object value = Convert.ChangeType(textBox.Text, parameterType);
                    parameters.Add(value);
                }

                object result = selectedMethod.Invoke(_createdInstance, parameters.ToArray());

                if (result != null)
                {
                    MessageBox.Show("Method execution result: " + result.ToString());
                }
                else
                {
                    MessageBox.Show("Method executed successfully.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error executing method: " + ex.Message);
            }
        }
    }
}