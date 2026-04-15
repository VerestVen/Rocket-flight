using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;        
using System.Windows.Shapes;
using System.Windows.Media.Effects;

#nullable disable
namespace Rocket
{
    // ================================================================
    // КЛАСС: ОКНО ДЛЯ ОТОБРАЖЕНИЯ РЕЗУЛЬТАТОВ
    // ================================================================
    // Этот класс создает графическое окно с результатами полета
    // ================================================================
    class WinRocket
    {
        [STAThread]
        public static void WinRocketTool(double time, double distance, double height)
        {
            // Получаем параметры полета из расчетов
            double flightTime = time;         // Время полета в секундах
            double maxDistance = distance;    // Максимальная дальность в метрах
            double maxHeight = height;        // Максимальная высота в метрах

            // СОЗДАЕМ ГЛАВНОЕ ОКНО
            Window win = new Window();
            win.Title = "Результаты полета ракеты";                         // Заголовок окна
            win.Width = 1050;                                               // Ширина окна
            win.Height = 410;                                               // Высота окна
            win.WindowStartupLocation = WindowStartupLocation.CenterScreen; // Окно по центру экрана


            Grid mainGrid = new Grid(); // Слои в программе, для того чтобы картинку на фон поставить 

            try
            {
                Image backgroundImage = new Image();                        // Создаем пустую картинку для хранения 

                // Загружаем картинку 
                BitmapImage bitmap = new BitmapImage();                     // Загрузчик
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("Rocket.png", UriKind.Relative);
                bitmap.EndInit();
                backgroundImage.Source = bitmap;                            // Помещаем картинку в наш контейнер
                backgroundImage.Stretch = Stretch.UniformToFill;            // Растягиваем 
                // backgroundImage.Opacity = 0.6;                           // Прозрачность 

                mainGrid.Children.Add(backgroundImage);                     // Делаем картинку фоном 
            }
            catch
            {
                // Если картинка не загрузилась, просто делаем белым фон
                mainGrid.Background = new SolidColorBrush(Colors.White);
            }

            // СОЗДАЕМ КОНТЕЙНЕР ДЛЯ ЭЛЕМЕНТОВ
            // StackPanel располагает элементы вертикально друг под другом
            var stackPanel = new System.Windows.Controls.StackPanel();
            stackPanel.Margin = new System.Windows.Thickness(30);           // Отступы от краев окна

            // ЗАГОЛОВОК 
            var titleText = new System.Windows.Controls.TextBlock();                    // Создаем объект для отображения текста
            titleText.Text = "РЕЗУЛЬТАТЫ ПОЛЕТА РАКЕТЫ";                                // Текст
            titleText.FontSize = 28;                                                    // Шрифт
            titleText.FontWeight = System.Windows.FontWeights.Bold;                     // Жирный
            titleText.Margin = new System.Windows.Thickness(0, 0, 0, 20);               // Отступы
            titleText.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;  // По центру
            stackPanel.Children.Add(titleText);                                         // Добавление

            // Время полета
            var timeText = new System.Windows.Controls.TextBlock();                     // Создаем объект для отображения текста
            timeText.Text = $"  Время полета: {flightTime:F2} секунд";                  // Текст
            timeText.FontSize = 16;                                                     // Шрифт
            timeText.FontWeight = System.Windows.FontWeights.SemiBold;                  // Слабее
            timeText.Margin = new System.Windows.Thickness(5, 5, 5, 5);                 // Отступы
            stackPanel.Children.Add(timeText);                                          // Добавление

            // Максимальная дальность
            var distanceText = new System.Windows.Controls.TextBlock();                 // Создаем объект для отображения текста
            distanceText.Text = $"  Максимальная дальность: {maxDistance:F2} метров";   // Текст
            distanceText.FontSize = 16;                                                 // Шрифт
            distanceText.FontWeight = System.Windows.FontWeights.SemiBold;              // Слабее
            distanceText.Margin = new System.Windows.Thickness(5, 5, 5, 5);             // Отступы
            stackPanel.Children.Add(distanceText);                                      // Добавление

            // Максимальная высота
            var heightText = new System.Windows.Controls.TextBlock();                   // Создаем объект для отображения текста         
            heightText.Text = $"  Максимальная высота: {maxHeight:F2} метров";          // Текст
            heightText.FontSize = 16;                                                   // Шрифт
            heightText.FontWeight = System.Windows.FontWeights.SemiBold;                // Слабее
            heightText.Margin = new System.Windows.Thickness(5, 5, 5, 5);               // Отступы
            stackPanel.Children.Add(heightText);                                        // Добавление


            // КНОПКА ЗАКРЫТИЯ 
            var okButton = new System.Windows.Controls.Button();                        // Создаем объект кнопки
            okButton.Content = "Закрыть";                                               // Текст
            okButton.Width = 150;                                                       // Ширина
            okButton.Height = 45;                                                       // Высота
            okButton.Margin = new System.Windows.Thickness(5, 10, 5, 10);               // Отступы
            okButton.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;   // По центру
            okButton.FontSize = 14;                                                     // Шрифт
            okButton.FontWeight = System.Windows.FontWeights.Bold;                      // Жирный

            // Обработчик нажатия кнопки - закрывает окно
            okButton.Click += (sender, e) =>
            {
                win.Close();
            };

            stackPanel.Children.Add(okButton); // Добавляем кнопку

            // Устанавливаем содержимое окна
            mainGrid.Children.Add(stackPanel);
            win.Content = mainGrid;

            // ЗАПУСКАЕМ ОКНО WPF
            win.Show();
            win.Activate();

        }
    }

    // ================================================================
    // КЛАСС: ОКНО ДЛЯ АНИМАЦИИ ПОЛЕТА РАКЕТЫ
    // ================================================================
    // Это окно визуализирует движение ракеты по траектории
    // ================================================================
    class RocketAnimationWindow : Window
    {
        private Canvas animationCanvas;                         // Холст для рисования
        private UIElement rocketImage;                              // Изображение ракеты
        private List<Ellipse> trailPoints;                      // След от ракеты
        private double[,] trajectory;                           // Траектория полета
        private int currentPoint;                               // Текущая точка траектории
        private System.Windows.Threading.DispatcherTimer timer; // Таймер анимации
        private double maxX, maxY;                              // Максимальные координаты
        private double canvasWidth, canvasHeight;               // Размеры холста
        private double scaleX, scaleY;                          // Масштаб для отображения
        private Polyline trajectoryLine;                        // Линия траектории
        private TextBlock infoText;                             // Текст с информацией
        private bool isPlaying = true;                          // Состояние анимации
        private Button playPauseButton;                         // Кнопка Play/Pause
        private Button resetButton;                             // Кнопка сброса
        private Slider progressSlider;                          // Ползунок прогресса
        private double totalTime;                               // Общее время полета
        private int totalPoints;                                // Всего точек траектории
        private Image backgroundImage;                          // Фоновое изображение

        [STAThread]
        public static void ShowAnimation(double[,] trajectoryData, double maxXCoord, double maxYCoord, double flightTime) // Создание окна
        {
            RocketAnimationWindow window = new RocketAnimationWindow(trajectoryData, maxXCoord, maxYCoord, flightTime);
            window.Show();
            window.Activate();
        }

        // КОНСТРУКТОР 
        public RocketAnimationWindow(double[,] trajectoryData, double maxXCoord, double maxYCoord, double flightTime)
        {
            trajectory = trajectoryData;                // Матрица координат (Траектория)
            maxX = maxXCoord;                           // Макс. по Х (Дальность)
            maxY = maxYCoord;                           // Макс. по Х (Высота)
            totalTime = flightTime;                     // Время полета 
            totalPoints = trajectoryData.GetLength(0);  // Кол. точек 
            currentPoint = 0;                           // Начальная точка 
            trailPoints = new List<Ellipse>();          // Лист для следа 

            InitializeWindow();                         // Создаем окно 
            InitializeCanvas();                         // Создаем область рисования
            DrawTrajectoryLine();                       // Рисуем пунктирный путь
            CreateRocketImage();                        // Ставим ракету на старт 
            StartAnimation();                           // Запускаем анимацию 
        }

        // СОЗДАЕМ ГЛАВНОЕ ОКНО
        private void InitializeWindow()
        {
            this.Title = "Анимация полета ракеты";                           // Заголовок
            this.Width = 1300;                                               // Ширина
            this.Height = 900;                                               // Высота
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen; // По центру

            // Создаем основной Grid
            Grid mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Анимация 
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });                      // Панель 
            this.Content = mainGrid;

            // Холст для анимации
            animationCanvas = new Canvas(); // Панель

            try
            {   // Загружаем фон картинки 
                backgroundImage = new Image();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("background.png", UriKind.Relative);
                bitmap.EndInit();
                backgroundImage.Source = bitmap;
                backgroundImage.Stretch = Stretch.UniformToFill;
                animationCanvas.Children.Add(backgroundImage);

                // Обновляем размер фона при изменении размера окна
                animationCanvas.SizeChanged += (s, e) =>
                {
                    if (backgroundImage != null)
                    {
                        Canvas.SetLeft(backgroundImage, 0);
                        Canvas.SetTop(backgroundImage, 0);
                        backgroundImage.Width = animationCanvas.ActualWidth;
                        backgroundImage.Height = animationCanvas.ActualHeight;
                    }
                };
            }
            catch
            {
                // Если фон не загрузился, используем голубой цвет
                animationCanvas.Background = new SolidColorBrush(Color.FromRgb(135, 206, 235));
            }

            animationCanvas.Margin = new Thickness(5, 5, 5, 5);  // Отступы
            Grid.SetRow(animationCanvas, 0);                     // Перемещаем на первое место 
            mainGrid.Children.Add(animationCanvas);              // Добавляем

            // Панель управления
            StackPanel controlPanel = CreateControlPanel();      // Создаем панель управления
            Grid.SetRow(controlPanel, 1);                        // Перемещаем на второе место 
            mainGrid.Children.Add(controlPanel);                 // Добавляем
        }

        // МЕНЯЕМ РАЗМЕРЫ ФОНА 
        private void InitializeCanvas()
        {
            animationCanvas.SizeChanged += (s, e) =>
            {
                // Получение новых размеров
                canvasWidth = animationCanvas.ActualWidth;
                canvasHeight = animationCanvas.ActualHeight;
                CalculateScale();           // Пересчет масштаба
                RedrawTrajectory();         // Перерисовка траектории
                UpdateRocketPosition();     // Обновление позиции ракеты

                // Обновляем размер фона
                if (backgroundImage != null)
                {
                    backgroundImage.Width = canvasWidth;
                    backgroundImage.Height = canvasHeight;
                }
            };
        }

        // МАСШТАБ ОТОБРАЖЕНИЯ
        private void CalculateScale()
        {
            // Добавляем отступы для лучшего отображения
            double marginX = canvasWidth * 0.005;
            double marginY = canvasHeight * 0.005;

            // Вычисление масштаба по X и Y
            scaleX = (canvasWidth - 2 * marginX) / maxX;
            scaleY = (canvasHeight - 2 * marginY) / maxY;

            // Выравнивание пропорций
            double scale = Math.Min(scaleX, scaleY);
            scaleX = scale;
            scaleY = scale;
        }

        // ПРЕОБРПЗОВАНИЕ РЕАЛЬНЫХ КООРДИНАТ
        private double ConvertX(double x)
        {
            double marginX = canvasWidth * 0.05;
            return marginX + x * scaleX;
        }
        private double ConvertY(double y)
        {
            double marginY = canvasHeight * 0.05;
            return canvasHeight - marginY - y * scaleY; //  ( т.k. ось Y растет вниз в WPF )
        }

        // ТРАЕКТОРИЯ
        private void DrawTrajectoryLine()
        {
            trajectoryLine = new Polyline();                                // Создание линии
            trajectoryLine.Stroke = new SolidColorBrush(Colors.Black);      // Цвет черный
            trajectoryLine.StrokeThickness = 1;                             // Толщина
            PointCollection points = new PointCollection();                 // Сбор точек траектории
            for (int i = 0; i < totalPoints; i++)
            {
                if (trajectory[i, 3] >= 0)  // Только точки с Y >= 0
                {
                    points.Add(new Point(ConvertX(trajectory[i, 2]), ConvertY(trajectory[i, 3])));
                }
            }
            trajectoryLine.Points = points;               // Установка точек
            animationCanvas.Children.Add(trajectoryLine); // Добавление на холст
        }

        // Перерисовываем траекторию при изменении размера окна
        private void RedrawTrajectory()
        {
            PointCollection points = new PointCollection();
            for (int i = 0; i < totalPoints; i++)
            {
                if (trajectory[i, 3] >= 0)
                {
                    points.Add(new Point(ConvertX(trajectory[i, 2]), ConvertY(trajectory[i, 3])));
                }
            }
            trajectoryLine.Points = points;
        }

        // Создание  изображения ракеты
        // Создание изображения ракеты
        private void CreateRocketImage()
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("Rocket1.png", UriKind.Relative);
                bitmap.EndInit();

                Image image = new Image();
                image.Source = bitmap;
                image.Width = 40;
                image.Height = 40;
                image.RenderTransformOrigin = new Point(0.5, 0.5);
                rocketImage = image;
            }
            catch
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Width = 12;
                ellipse.Height = 12;
                ellipse.Fill = new SolidColorBrush(Colors.Red);
                ellipse.Stroke = new SolidColorBrush(Colors.DarkRed);
                ellipse.StrokeThickness = 1;
                ellipse.RenderTransformOrigin = new Point(0.5, 0.5);
                rocketImage = ellipse;
            }

            animationCanvas.Children.Add(rocketImage);
        }

        private StackPanel CreateControlPanel()
        {
            // Создаем рамки для фона и отступов
            Border controlBorder = new Border();
            controlBorder.Background = new SolidColorBrush(Color.FromArgb(0, 150, 150, 50));
            controlBorder.Margin = new Thickness(5);  // Отступы
            controlBorder.Padding = new Thickness(5); // Внутренние отступы

            StackPanel panel = new StackPanel();      // Панель
            panel.Orientation = Orientation.Vertical; // Элементы сверху вниз

            // Ползунок прогресса
            progressSlider = new Slider();  
            progressSlider.Minimum = 0;                          // MIN диапозон
            progressSlider.Maximum = totalPoints - 1;            // MAX диапозон
            progressSlider.Value = 0;                            // Старт
            progressSlider.Margin = new Thickness(5, 5, 5, 5);   // Отступы
            progressSlider.ValueChanged += (s, e) =>
            {
                if (!isPlaying)  // Когда анимация не идет
                {
                    currentPoint = (int)e.NewValue; // Устанавливаем текущую точку
                    UpdateRocketPosition();         // Перемещаем ракету
                    UpdateInfoText();               // Обновляем информацию
                }
            };
            panel.Children.Add(progressSlider);     // Добавление

            // Панель кнопок
            StackPanel buttonPanel = new StackPanel();
            buttonPanel.Orientation = Orientation.Horizontal;               // Кнопки в ряд
            buttonPanel.HorizontalAlignment = HorizontalAlignment.Center;   // По центру

            // Кнопка Play/Pause
            playPauseButton = new Button();
            playPauseButton.Content = "⏸ Пауза";
            playPauseButton.Width = 150;
            playPauseButton.Height = 45;
            playPauseButton.Margin = new Thickness(5);
            playPauseButton.FontSize = 14;
            playPauseButton.Click += (s, e) => TogglePlayPause();
            buttonPanel.Children.Add(playPauseButton);

            // Кнопка сброса
            resetButton = new Button();
            resetButton.Content = "⟳ Сброс";
            resetButton.Width = 150;
            resetButton.Height = 45;
            resetButton.Margin = new Thickness(5);
            resetButton.FontSize = 14;
            resetButton.Click += (s, e) => ResetAnimation();
            buttonPanel.Children.Add(resetButton);

            // Кнопка скорости
            Button speedButton = new Button();
            speedButton.Content = "⚡ Скорость x10";
            speedButton.Width = 150;
            speedButton.Height = 45;
            speedButton.Margin = new Thickness(5);
            speedButton.FontSize = 14;
            speedButton.Click += (s, e) => ChangeSpeed();
            buttonPanel.Children.Add(speedButton);

            // Кнопка закрыть
            Button okButton = new Button();
            okButton.Content = "X Закрыть";
            okButton.Width = 150;
            okButton.Height = 45;
            okButton.Margin = new Thickness(5);
            okButton.FontSize = 14;

            // Обработчик нажатия кнопки - закрывает окно
            okButton.Click += (sender, e) =>
            {
                this.Close();  
            };
            buttonPanel.Children.Add(okButton);

            panel.Children.Add(buttonPanel);

            // Информационный текст
            infoText = new TextBlock();
            infoText.Foreground = new SolidColorBrush(Colors.Green);
            infoText.FontSize = 12;
            infoText.TextAlignment = TextAlignment.Center;
            infoText.Margin = new Thickness(10);
            panel.Children.Add(infoText);

            controlBorder.Child = panel;

            // Создаем StackPanel для возврата, содержащий Border
            StackPanel resultPanel = new StackPanel();
            resultPanel.Children.Add(controlBorder);
            return resultPanel;
        }

        private void StartAnimation()
        {
            timer = new System.Windows.Threading.DispatcherTimer();   // Создвание таймера
            timer.Interval = TimeSpan.FromMilliseconds(50);           // 20 FPS  =  1000 мс / 50 мс
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!isPlaying) return;

            if (currentPoint < totalPoints - 1)
            {
                currentPoint++;                         // Переход к следующей точке
                progressSlider.Value = currentPoint;    // Обновляем ползунок
                UpdateRocketPosition();                 // Перемещаем ракету
                UpdateInfoText();                       // Обновляем информацию
            }
            else
            {
                // Анимация завершена
                timer.Stop();
                playPauseButton.Content = "▶ Старт";
                isPlaying = false;
                MessageBox.Show("Полет завершен!", "Анимация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Перемещаем ракету в нужную точку и делаем поворот
        private void UpdateRocketPosition()
        {
            if (currentPoint >= totalPoints) return;

            double x = trajectory[currentPoint, 2];
            double y = trajectory[currentPoint, 3];

            if (y < 0) y = 0;

            double canvasX = ConvertX(x);
            double canvasY = ConvertY(y);

            // Определяем размеры для центрирования
            double halfWidth = 6;
            double halfHeight = 6;

            if (rocketImage is Image img)
            {
                halfWidth = img.Width / 2;
                halfHeight = img.Height / 2;
            }
            else if (rocketImage is Ellipse ellipse)
            {
                halfWidth = ellipse.Width / 2;
                halfHeight = ellipse.Height / 2;
            }

            Canvas.SetLeft(rocketImage, canvasX - halfWidth);
            Canvas.SetTop(rocketImage, canvasY - halfHeight);

            RotateRocket();
        }

        // ПОВОРОТ РАКЕТЫ ПО НАПРАВЛЕНИЮ ДВИЖЕНИЯ 
        private void RotateRocket()
        {
            if (currentPoint > 0 && currentPoint < totalPoints)
            {
                double x1 = trajectory[currentPoint - 1, 2];
                double y1 = trajectory[currentPoint - 1, 3];
                double x2 = trajectory[currentPoint, 2];
                double y2 = trajectory[currentPoint, 3];

                double dx = x2 - x1;
                double dy = y2 - y1;

                double angleDegrees = 0;

                if (dx != 0 || dy != 0)
                {
                    // Угол в радианах, затем в градусы
                    double angleRadians = -(Math.Atan2(dy, dx));
                    angleDegrees = (angleRadians * 180 / Math.PI) + 90;
                }
                RotateTransform rotate = new RotateTransform(angleDegrees);
                rocketImage.RenderTransform = rotate;
            }
        }

        // ТЕКУЩАЯ ИНФОРМАЦИЯ
        private void UpdateInfoText()
        {
            if (currentPoint < totalPoints)
            {
                double time = trajectory[currentPoint, 0];
                double speed = trajectory[currentPoint, 1];
                double x = trajectory[currentPoint, 2];
                double y = trajectory[currentPoint, 3];

                infoText.Text = string.Format(
                    "⏱ Время: {0:F2} сек | 🚀 Скорость: {1:F1} м/с | 📍 X: {2:F1} м | 📍 Y: {3:F1} м | 📊 Прогресс: {4:F1}%",
                    time, speed, x, Math.Max(0, y), (double)currentPoint / totalPoints * 100);
            }
        }

        private void TogglePlayPause()
        {
            isPlaying = !isPlaying;
            playPauseButton.Content = isPlaying ? "⏸ Пауза" : "▶ Старт";

            if (isPlaying && currentPoint < totalPoints - 1)
            {
                if (!timer.IsEnabled)
                    timer.Start();
            }
        }

        private void ResetAnimation()
        {
            isPlaying = false;
            playPauseButton.Content = "▶ Старт";
            currentPoint = 0;
            progressSlider.Value = 0;

            // Очищаем след
            foreach (var dot in trailPoints)
            {
                animationCanvas.Children.Remove(dot);
            }
            trailPoints.Clear();

            UpdateRocketPosition();
            UpdateInfoText();

            if (!timer.IsEnabled)
                timer.Start();
            else
                isPlaying = false;
        }

        private void ChangeSpeed()
        {
            if (timer.Interval == TimeSpan.FromMilliseconds(50))
            {
                timer.Interval = TimeSpan.FromMilliseconds(5);
                playPauseButton.Content = isPlaying ? "⏸ Пауза (x10)" : "▶ Старт (x10)";
            }
            else
            {
                timer.Interval = TimeSpan.FromMilliseconds(50);
                playPauseButton.Content = isPlaying ? "⏸ Пауза" : "▶ Старт";
            }
        }
    }

    // ================================================================
    // ПРОСТОЕ ОКНО ДЛЯ ВВОДА ПАРАМЕТРОВ
    // ================================================================
    public class InputDialog : Window
    {
        public double InputAngle { get; private set; }
        public double InputTime { get; private set; }
        public double InputVelocity { get; private set; }
        public double InputHeight { get; private set; }
        public double InputDrag { get; private set; }
        public double InputMass { get; private set; }
        public double InputArea { get; private set; }
        public bool IsConfirmed { get; private set; } = false;

        public InputDialog()
        {
            this.Title = "Ввод параметров полета";
            this.Width = 450;
            this.Height = 750;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Создаем StackPanel для вертикального расположения
            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(20);

            // Заголовок
            panel.Children.Add(new TextBlock
            {
                Text = "ВВЕДИТЕ ПАРАМЕТРЫ ПОЛЕТА",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            });

            // Поля ввода
            var angleBox = AddInputField(panel, "Угол броска (0-90°):", "45");
            var timeBox = AddInputField(panel, "Время полета (сек):", "10");
            var velocityBox = AddInputField(panel, "Начальная скорость (м/с):", "100");
            var heightBox = AddInputField(panel, "Начальная высота (м):", "0");
            var dragBox = AddInputField(panel, "Сопротивление (0-1):", "0");
            var massBox = AddInputField(panel, "Масса (кг):", "1");
            var areaBox = AddInputField(panel, "Площадь (м²):", "0.1");

            // Кнопки
            StackPanel buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0)
            };

            Button okButton = new Button { Content = "OK", Width = 100, Height = 35, Margin = new Thickness(5) };
            okButton.Click += (s, e) =>
            {
                // Проверяем и сохраняем значения
                if (!double.TryParse(angleBox.Text, out double angle) || angle < 0 || angle > 90)
                { MessageBox.Show("Угол должен быть от 0 до 90!"); return; }
                if (!double.TryParse(timeBox.Text, out double time) || time <= 0)
                { MessageBox.Show("Время должно быть положительным!"); return; }
                if (!double.TryParse(velocityBox.Text, out double velocity) || velocity <= 0)
                { MessageBox.Show("Скорость должна быть положительной!"); return; }
                if (!double.TryParse(heightBox.Text, out double height) || height < 0)
                { MessageBox.Show("Высота должна быть >= 0!"); return; }
                if (!double.TryParse(dragBox.Text, out double drag) || drag < 0)
                { MessageBox.Show("Сопротивление должно быть >= 0!"); return; }
                if (!double.TryParse(massBox.Text, out double mass) || mass <= 0)
                { MessageBox.Show("Масса должна быть положительной!"); return; }
                if (!double.TryParse(areaBox.Text, out double area) || area <= 0)
                { MessageBox.Show("Площадь должна быть положительной!"); return; }

                InputAngle = angle;
                InputTime = time;
                InputVelocity = velocity;
                InputHeight = height;
                InputDrag = drag;
                InputMass = mass;
                InputArea = area;
                IsConfirmed = true;

                // Устанавливаем DialogResult = true
                this.DialogResult = true;
                this.Close();
            };

            Button cancelButton = new Button { Content = "Отмена", Width = 100, Height = 35, Margin = new Thickness(5) };
            cancelButton.Click += (s, e) =>
            {
                this.DialogResult = false;
                this.Close();
            };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);
            panel.Children.Add(buttonPanel);

            this.Content = panel;
        }

        private TextBox AddInputField(StackPanel panel, string label, string defaultValue)
        {
            // Создаем группу для каждого поля
            StackPanel fieldPanel = new StackPanel { Margin = new Thickness(0, 5, 0, 5) };

            fieldPanel.Children.Add(new TextBlock
            {
                Text = label,
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 3)
            });

            TextBox textBox = new TextBox
            {
                Text = defaultValue,
                FontSize = 14,
                Height = 30,
                Padding = new Thickness(5)
            };

            fieldPanel.Children.Add(textBox);
            panel.Children.Add(fieldPanel);

            return textBox;
        }
    }

    // ================================================================
    // КЛАСС: РАКЕТА 
    // ================================================================
    // Этот класс рассчитывает траекторию полета с учетом
    // сопротивления воздуха и гравитации
    // ================================================================
    class Rocket
    {
        //  ДЕЛЕГАТЫ И СОБЫТИЯ 
        public delegate void RocketFallHandler(bool isFallen);
        public event RocketFallHandler RocketFall;

        //  ФИЗИЧЕСКИЕ КОНСТАНТЫ 
        private const double G = 9.80665;           // Ускорение свободного падения (м/с²)
        private const double AirDensity = 1.225;    // Плотность воздуха (кг/м³)

        //  ПАРАМЕТРЫ ПОЛЕТА 
        private double time;                        // Максимальное время расчета (секунды)
        private double velocity;                    // Начальная скорость (м/с)
        private double height;                      // Начальная высота (м)
        private double angleDeg;                    // Угол в градусах
        private double angleRad;                    // Угол в радианах

        //  ПАРАМЕТРЫ СОПРОТИВЛЕНИЯ ВОЗДУХА 
        private double dragCoefficient;             // Коэффициент сопротивления (0 - нет, 0.1-1.0 типичные)
        private double mass;                        // Масса объекта (кг)
        private double crossSectionArea;            // Площадь поперечного сечения (м²)

        //  ПАРАМЕТРЫ ДИСКРЕТИЗАЦИИ 
        private int N;                              // Количество точек разбиения (автоматически)
        private double[,] coordinatesMatrix;        // Матрица для хранения: [время, скорость, X, Y]

        //  РЕЗУЛЬТАТЫ РАСЧЕТА 
        public double maxX;                         // Максимальная дальность (м)
        public double maxY;                         // Максимальная высота (м)
        public double maxTime;                      // Полное время полета (с)

        // ------------------------------------------------------------
        // КОНСТРУКТОР КЛАССА ROCKET
        // ------------------------------------------------------------
        // a: угол в градусах (0-90)
        // t: максимальное время расчета (с)
        // v0: начальная скорость (м/с)
        // h: начальная высота (м)
        // dragCoef: коэффициент сопротивления (0 - нет сопротивления)
        // massKg: масса объекта (кг)
        // area: площадь поперечного сечения (м²)
        // ------------------------------------------------------------
        public Rocket(double a, double t, double v0, double h, double dragCoef = 0, double massKg = 1, double area = 0.1)
        {
            // Сохраняем параметры полета
            angleDeg = a;
            time = t;
            velocity = v0;
            height = h;

            // Сохраняем параметры сопротивления
            dragCoefficient = dragCoef;
            mass = massKg;
            crossSectionArea = area;

            // Переводим угол из градусов в радианы (для тригонометрических функций)
            angleRad = angleDeg * Math.PI / 180.0;

            // АВТОМАТИЧЕСКИЙ ВЫБОР ОПТИМАЛЬНОГО КОЛИЧЕСТВА ТОЧЕК
            N = CalculateOptimalN();

            // Для отладки (можно закомментировать)
            Console.WriteLine($" Автоматически выбрано N = {N} точек разбиения");

            coordinatesMatrix = new double[N, 4];
        }

        private int CalculateOptimalN()
        {
            // ОЦЕНИВАЕМ ВРЕМЯ ПОЛЕТА (без точного расчета)
            double v0y = velocity * Math.Sin(angleRad); // Вертикальная составляющая скорости
            double estimatedTime;                       // Оценочное время полета 

            if (dragCoefficient == 0)
            {
                // Точная формула для случая без сопротивления
                double discriminant = v0y * v0y + 2 * G * height;
                estimatedTime = (v0y + Math.Sqrt(discriminant)) / G;
            }
            else
            {
                // Оценка с сопротивлением 
                double idealTime = (v0y + Math.Sqrt(v0y * v0y + 2 * G * height)) / G;
                estimatedTime = idealTime * 0.7;  // Примерная оценка
            }


            //  Время полета (30 точек в секунду - достаточно для гладкости)
            int timeComponent = Math.Max(100, (int)(estimatedTime * 30));

            // Скорость (быстрые объекты требуют больше точек)
            // Чем выше скорость, тем быстрее меняется положение, коэф 5 меняяем, чем меньше число тем больше точек 
            int velocityComponent = (int)(velocity / 5);

            // Угол (крутые траектории требуют больше деталей в вершине)
            // Угол 45° - оптимальный, отклонение увеличивает сложность
            double angleFactor = Math.Abs(angleDeg - 45) / 45.0;  // 0 при 45°, 1 при 0° или 90°
            int angleComponent = (int)(200 * angleFactor);
            //                       200 точек 

            // Сопротивление воздуха (усложняет траекторию)
            int dragComponent = dragCoefficient > 0 ? (int)(dragCoefficient * 300) : 0;
            //              сопротивление воздуха ?                       300 точек 

            // Высота (чем выше, тем дольше полет)
            // Каждые 10 метров добавляем точку 
            int heightComponent = (int)(height / 10);

            //СУММИРУЕМ И ОГРАНИЧИВАЕМ
            int N = timeComponent + velocityComponent + angleComponent + dragComponent + heightComponent;

            // Минимум  300  точек (для плавной визуализации)
            // Максимум 3000 точек (чтобы не тормозить)
            N = Math.Clamp(N, 300, 3000);

            // ОКРУГЛЯЕМ 
            if (N <= 500) return 500;
            if (N <= 1000) return 1000;
            if (N <= 1500) return 1500;
            if (N <= 2000) return 2000;
            else return 3000;
        }

        // Создаем матрицу для хранения результатов
        // Размер: [N, 4] где 4 колонки: Время, Скорость, X, Y
        // coordinatesMatrix = new double[N, 4];

        // ------------------------------------------------------------
        // РАСЧЕТ СИЛЫ СОПРОТИВЛЕНИЯ ВОЗДУХА
        // ------------------------------------------------------------
        // Формула: F = 0.5 * ρ * C * A * v²
        // где:
        //   ρ - плотность воздуха
        //   C - коэффициент сопротивления
        //   A - площадь сечения
        //   v - скорость
        // ------------------------------------------------------------
        private double CalculateDragForce(double speed)
        {
            // Если скорость 0 или нет сопротивления, сила равна 0
            if (speed <= 0 || dragCoefficient == 0) return 0;

            // Классическая формула аэродинамического сопротивления
            return 0.5 * AirDensity * dragCoefficient * crossSectionArea * speed * speed;
        }

        // ------------------------------------------------------------
        //  РАСЧЕТ УСКОРЕНИЯ ОТ СОПРОТИВЛЕНИЯ
        // ------------------------------------------------------------
        // По второму закону Ньютона: a = F / m
        // ------------------------------------------------------------
        private double CalculateDragAcceleration(double speed)
        {
            if (mass <= 0) return 0;  // Защита от деления на ноль
            return CalculateDragForce(speed) / mass;
        }

        // ------------------------------------------------------------
        //  РАСЧЕТ ВРЕМЕНИ ПАДЕНИЯ (АНАЛИТИЧЕСКИЙ, БЕЗ СОПРОТИВЛЕНИЯ)
        // ------------------------------------------------------------
        // Используется только когда dragCoefficient = 0
        // Решаем квадратное уравнение: h + v0*sin(θ)*t - g*t²/2 = 0
        // ------------------------------------------------------------
        private double CalculateFallTimeNoDrag()
        {
            double v0y = velocity * Math.Sin(angleRad);         // Вертикальная составляющая скорости
            double discriminant = v0y * v0y + 2 * G * height;   // Дискриминант квадратного уравнения
            if (discriminant < 0) return 0;                     // Нет решения (не должно произойти)
            else return (v0y + Math.Sqrt(discriminant)) / G;    // Положительный корень квадратного уравнения
        }

        // ------------------------------------------------------------
        //  РАСЧЕТ ТРАЕКТОРИИ С УЧЕТОМ СОПРОТИВЛЕНИЯ
        // ------------------------------------------------------------
        // Использует численное интегрирование (метод Эйлера)
        // Разбивает полет на маленькие шаги и на каждом шаге
        // пересчитывает скорость и положение
        // ------------------------------------------------------------
        private void CalculateTrajectoryWithDrag()
        {
            // Находим время падения
            double fallTime = CalculateFallTimeWithDrag();
            maxTime = fallTime;

            // Шаг по времени (dt = общее время / количество точек)
            double dt = fallTime / N;

            // Начальные условия
            double x = 0;                               // Начальная координата X
            double y = height;                          // Начальная координата Y
            double vx = velocity * Math.Cos(angleRad);  // Горизонтальная скорость
            double vy = velocity * Math.Sin(angleRad);  // Вертикальная скорость

            maxX = 0;
            maxY = height;

            // ПОШАГОВЫЙ РАСЧЕТ ТРАЕКТОРИИ
            for (int i = 0; i < N; i++)
            {
                double currentTime = (i + 1) * dt;

                // Если тело уже упало, прерываем расчет
                if (y <= 0 && currentTime > 0)
                {
                    // Фиксируем момент касания земли
                    y = 0;
                    coordinatesMatrix[i, 0] = currentTime;
                    coordinatesMatrix[i, 1] = Math.Sqrt(vx * vx + vy * vy);
                    coordinatesMatrix[i, 2] = x;
                    coordinatesMatrix[i, 3] = y;
                    maxTime = currentTime;
                    break;
                }

                // ТЕКУЩАЯ СКОРОСТЬ (модуль вектора скорости)
                double speed = Math.Sqrt(vx * vx + vy * vy);

                // УСКОРЕНИЕ ОТ СОПРОТИВЛЕНИЯ
                double dragAcc = CalculateDragAcceleration(speed);

                if (speed > 0)  // Если скорость не нулевая
                {
                    // Сопротивление действует ПРОТИВ направления движения
                    // Разлагаем ускорение на компоненты X и Y
                    double dragAx = -dragAcc * (vx / speed);
                    double dragAy = -dragAcc * (vy / speed);

                    // Обновляем скорость с учетом:
                    // - сопротивления воздуха (dragAx, dragAy)
                    // - гравитации (только для Y: -G)
                    vx += dragAx * dt;
                    vy += (dragAy - G) * dt;
                }
                else
                {
                    // Если скорость нулевая, добавляем только гравитацию
                    vy -= G * dt;
                }

                // Обновляем координаты
                x += vx * dt;
                y += vy * dt;

                // Сохраняем результаты в матрицу
                coordinatesMatrix[i, 0] = currentTime;                     // Время
                coordinatesMatrix[i, 1] = Math.Sqrt(vx * vx + vy * vy);    // Скорость (модуль)
                coordinatesMatrix[i, 2] = x;                               // Координата X
                coordinatesMatrix[i, 3] = y;                               // Координата Y

                // Обновляем максимальные значения
                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;
            }

            // Вызываем событие о падении
            RocketFall?.Invoke(true);
        }

        // ------------------------------------------------------------
        // РАСЧЕТ ВРЕМЕНИ ПАДЕНИЯ С УЧЕТОМ СОПРОТИВЛЕНИЯ
        // ------------------------------------------------------------
        // Использует численное интегрирование для нахождения момента
        // когда тело достигнет земли (y = 0)
        // ------------------------------------------------------------
        private double CalculateFallTimeWithDrag()
        {
            // Если нет сопротивления, используем аналитическую формулу
            if (dragCoefficient == 0)
            {
                return CalculateFallTimeNoDrag();
            }

            // Для сопротивления используем пошаговый расчет
            double dtSimulation = 0.005;                // Шаг симуляции (0.005 секунды = 5 мс)
            double currentTime = 0;                     // Текущее время полета (секунды)
            double x = 0;                               // Горизонтальная координата (метры)
            double y = height;                          // Вертикальная координата (метры)
            double vx = velocity * Math.Cos(angleRad);  // Горизонтальная скорость (м/с)
            double vy = velocity * Math.Sin(angleRad);  // Вертикальная скорость (м/с)  

            // Максимальное время симуляции (защита от бесконечного цикла)
            double maxSimulationTime = 1000;

            while (y >= 0 && currentTime < maxSimulationTime)       // Цикл выполняется, пока ракета не упала на землю (y >= 0) и не превышено максимальное время.
            {
                double speed = Math.Sqrt(vx * vx + vy * vy);        // Модуль вектора скорости
                double dragAcc = CalculateDragAcceleration(speed);  // Ускорение торможения

                if (speed > 0)
                {
                    double dragAx = -dragAcc * (vx / speed);        // Горизонтальная составляющая
                    double dragAy = -dragAcc * (vy / speed);        // Вертикальная составляющая

                    vx += dragAx * dtSimulation;                    // Обновление горизонтальной скорости
                    vy += (dragAy - G) * dtSimulation;              // Обновление вертикальной скорости
                }
                else
                {
                    vy -= G * dtSimulation;
                }

                x += vx * dtSimulation;         // Новое положение по X = старое + скорость × время
                y += vy * dtSimulation;         // Новое положение по Y
                currentTime += dtSimulation;    // Увеличиваем время на шаг
            }

            return currentTime;     // Возвращается полное время полета (когда ракета достигла земли)
        }

        // ------------------------------------------------------------
        // РАСЧЕТ ТРАЕКТОРИИ (БЕЗ СОПРОТИВЛЕНИЯ)
        // ------------------------------------------------------------
        // Аналитическое решение - точные формулы для идеального случая
        // ------------------------------------------------------------
        private void CalculateTrajectoryNoDrag()
        {
            double fallTime = CalculateFallTimeNoDrag();

            if (time < fallTime)  // Тело не упало за заданное время
            {
                RocketFall?.Invoke(false); // Событие 
                maxTime = time;
                double dt = time / N;

                for (int i = 0; i < N; i++)
                {
                    double currentTime = (i + 1) * dt;
                    coordinatesMatrix[i, 0] = currentTime;
                    coordinatesMatrix[i, 1] = velocity * Math.Sin(angleRad) - G * currentTime;
                    coordinatesMatrix[i, 2] = velocity * currentTime * Math.Cos(angleRad);
                    coordinatesMatrix[i, 3] = height + velocity * currentTime * Math.Sin(angleRad) - (G * currentTime * currentTime / 2);
                }

                maxX = coordinatesMatrix[N - 1, 2];
                maxY = FindMaxY();
            }
            else  // Тело упало
            {
                RocketFall?.Invoke(true); // Событие 
                maxTime = fallTime;
                double dt = fallTime / N;

                for (int i = 0; i < N; i++)
                {
                    double currentTime = (i + 1) * dt;
                    coordinatesMatrix[i, 0] = currentTime;
                    coordinatesMatrix[i, 1] = velocity * Math.Sin(angleRad) - G * currentTime;
                    coordinatesMatrix[i, 2] = velocity * currentTime * Math.Cos(angleRad);
                    coordinatesMatrix[i, 3] = height + velocity * currentTime * Math.Sin(angleRad) - (G * currentTime * currentTime / 2);
                }

                maxX = coordinatesMatrix[N - 1, 2];
                maxY = FindMaxY();
            }
        }

        // ------------------------------------------------------------
        // НАХОЖДЕНИЕ МАКСИМАЛЬНОЙ ВЫСОТЫ
        // ------------------------------------------------------------
        private double FindMaxY()
        {
            double maxYValue = 0;
            for (int i = 0; i < N; i++)
            {
                if (coordinatesMatrix[i, 3] > maxYValue)
                    maxYValue = coordinatesMatrix[i, 3];
            }
            return maxYValue;
        }

        // ------------------------------------------------------------
        // ЗАПУСК РАСЧЕТА ТРАЕКТОРИИ
        // ------------------------------------------------------------
        // Автоматически выбирает метод расчета в зависимости от наличия
        // сопротивления воздуха
        // ------------------------------------------------------------
        public void CalculateTrajectory()
        {
            if (dragCoefficient == 0)
            {
                // Без сопротивления - быстрое аналитическое решение
                CalculateTrajectoryNoDrag();
            }
            else
            {
                // С сопротивлением - численное интегрирование
                CalculateTrajectoryWithDrag();
            }
        }

        // ------------------------------------------------------------
        // ВЫВОД ТАБЛИЦЫ РЕЗУЛЬТАТОВ
        // ------------------------------------------------------------
        public void PrintTable()
        {
            Console.WriteLine("\n" + new string('─', 61));
            Console.WriteLine("           ТАБЛИЦА ПАРАМЕТРОВ ПОЛЕТА");
            Console.WriteLine(new string('─', 61));
            Console.WriteLine("│ {0,-12} │ {1,-12} │ {2,-12} │ {3,-12} │",
                              "Время (с)", "Скорость", "X (м)", "Y (м)");
            Console.WriteLine(new string('─', 61));

            for (int i = 0; i < N; i++)
            {
                // Показываем каждую 10-ю точку, чтобы не засорять консоль
                if (i % 10 == 0 || i == N - 1)
                {
                    Console.WriteLine("│ {0,-12:F4} │ {1,-12:F4} │ {2,-12:F4} │ {3,-12:F4} │",
                        coordinatesMatrix[i, 0], coordinatesMatrix[i, 1],
                        coordinatesMatrix[i, 2], coordinatesMatrix[i, 3]);
                }
            }
            Console.WriteLine(new string('─', 61));
        }

        // ------------------------------------------------------------
        // ВИЗУАЛИЗАЦИЯ ТРАЕКТОРИИ В КОНСОЛИ
        // ------------------------------------------------------------
        public void DrawTrajectory()
        {
            Console.WriteLine("\n\n" + new string('─', 70));
            Console.WriteLine("              ТРАЕКТОРИЯ ПОЛЕТА");
            Console.WriteLine(new string('─', 70));

            // Находим максимальные значения для масштабирования
            maxX = 0;
            maxY = 0;

            for (int i = 0; i < N; i++)
            {
                if (coordinatesMatrix[i, 2] > maxX) maxX = coordinatesMatrix[i, 2];
                if (coordinatesMatrix[i, 3] > maxY) maxY = coordinatesMatrix[i, 3];
            }

            // Добавляем 10% запаса для лучшей видимости
            double drawMaxX = maxX * 1.1;
            double drawMaxY = Math.Max(maxY * 1.1, height * 1.1);

            // Размеры "экрана" в символах
            int width = 80;        // Ширина консоли в символах
            int heightCanvas = 25; // Высота консоли в символах

            // Создаем двумерный массив для рисования
            char[,] canvas = new char[heightCanvas, width];

            // Заполняем фон пробелами
            for (int i = 0; i < heightCanvas; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    canvas[i, j] = ' ';
                }
            }

            // РИСУЕМ СИСТЕМУ КООРДИНАТ
            int originX = 5;                    // Начало координат по X (в символах)
            int originY = heightCanvas - 3;     // Начало координат по Y (в символах)

            // Горизонтальная ось (X)
            for (int j = originX; j < width - 2; j++)
            {
                canvas[originY, j] = '─';
            }

            // Вертикальная ось (Y)
            for (int i = 2; i < heightCanvas - 2; i++)
            {
                canvas[i, originX - 1] = '│';
            }

            // Начало координат
            canvas[originY, originX - 1] = '└';
            canvas[originY, originX] = '─';

            // Подписи осей
            canvas[2, originX - 1] = 'Y';
            canvas[originY, width - 2] = 'X';

            // РИСУЕМ ТРАЕКТОРИЮ
            for (int i = 0; i < N; i++)
            {
                // Масштабируем координаты в символы консоли
                int xPos = originX + (int)((coordinatesMatrix[i, 2] / drawMaxX) * (width - originX - 8));
                int yPos = originY - (int)((coordinatesMatrix[i, 3] / drawMaxY) * (originY - 3));

                // Проверка границ массива
                if (xPos >= 0 && xPos < width && yPos >= 0 && yPos < heightCanvas)
                {
                    canvas[yPos, xPos] = '*';  // Точка траектории
                }
            }

            // РИСУЕМ ТОЧКУ СТАРТА
            int startX = originX;
            int startY = originY - (int)((height / drawMaxY) * (originY - 3));
            if (startX >= 0 && startX < width && startY >= 0 && startY < heightCanvas)
            {
                canvas[startY, startX] = 'S';  // S = Start (начало)
            }

            // ВЫВОДИМ ГРАФИК НА ЭКРАН
            for (int i = 0; i < heightCanvas; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Console.Write(canvas[i, j]);
                }
                Console.WriteLine();
            }

            // ЛЕГЕНДА И СТАТИСТИКА
            Console.WriteLine("\n" + new string('─', 70));
            Console.WriteLine(" ЛЕГЕНДА И СТАТИСТИКА ПОЛЕТА");
            Console.WriteLine(new string('─', 70));
            Console.WriteLine($"  * - точки траектории (каждая {maxTime / N:F2} секунд)");
            Console.WriteLine($"  S - точка старта (высота: {height:F2} м)");
            Console.WriteLine($"\n Максимальная высота:   {maxY:F2} метров");
            Console.WriteLine($" Максимальная дальность: {maxX:F2} метров");
            Console.WriteLine($" Полное время полета:    {maxTime:F2} секунд");
            Console.WriteLine($" Количество точек:       {N}");

            // Информация о сопротивлении воздуха
            if (dragCoefficient > 0)
            {
                Console.WriteLine($"\n   СОПРОТИВЛЕНИЕ ВОЗДУХА (АКТИВНО)");
                Console.WriteLine($" Коэффициент:    C = {dragCoefficient:F3}");
                Console.WriteLine($" Масса:          m = {mass:F2} кг");
                Console.WriteLine($" Площадь:        A = {crossSectionArea:F4} м²");
                Console.WriteLine($" Плотность:      ρ = {AirDensity} кг/м³");
            }
            else
            {
                Console.WriteLine($"\n   СОПРОТИВЛЕНИЕ ВОЗДУХА (ОТКЛЮЧЕНО)");
                Console.WriteLine($" Идеальная траектория без учета воздуха");
            }
            Console.WriteLine(new string('─', 70));
        }

        // ------------------------------------------------------------
        // СРАВНЕНИЕ С ИДЕАЛЬНОЙ ТРАЕКТОРИЕЙ
        // ------------------------------------------------------------
        public void CompareWithNoResistance()
        {
            if (dragCoefficient == 0)
            {
                Console.WriteLine("\n  Сравнение недоступно: сопротивление воздуха уже отключено.");
                return;
            }

            Console.WriteLine("\n" + new string('─', 70));
            Console.WriteLine("        СРАВНЕНИЕ: С СОПРОТИВЛЕНИЕМ vs БЕЗ СОПРОТИВЛЕНИЯ");
            Console.WriteLine(new string('─', 70));

            // Создаем ракету без сопротивления для сравнения
            Rocket rocketIdeal = new Rocket(angleDeg, maxTime, velocity, height, 0, 1, 0.1);
            rocketIdeal.CalculateTrajectory();

            Console.WriteLine($"\n ПАРАМЕТРЫ:");
            Console.WriteLine($"┌─────────────────────┬──────────────────┬──────────────────┬─────────────┐");
            Console.WriteLine($"│ Параметр            │Без сопротивления │С сопротивлением  │ Потеря      │");
            Console.WriteLine($"├─────────────────────┼──────────────────┼──────────────────┼─────────────┤");
            Console.WriteLine($"│ Макс. дальность (м) │ {rocketIdeal.maxX,16:F2} │ {maxX,16:F2} │ {(rocketIdeal.maxX - maxX) / rocketIdeal.maxX * 100,11:F1}%│");
            Console.WriteLine($"│ Макс. высота (м)    │ {rocketIdeal.maxY,16:F2} │ {maxY,16:F2} │ {(rocketIdeal.maxY - maxY) / rocketIdeal.maxY * 100,11:F1}%│");
            Console.WriteLine($"│ Время полета (с)    │ {rocketIdeal.maxTime,16:F2} │ {maxTime,16:F2} │ {(rocketIdeal.maxTime - maxTime) / rocketIdeal.maxTime * 100,11:F1}%│");
            Console.WriteLine($"└─────────────────────┴──────────────────┴──────────────────┴─────────────┘");

            Console.WriteLine($"\n ВЫВОД: Сопротивление воздуха уменьшило:");
            Console.WriteLine($"   • Дальность на {(rocketIdeal.maxX - maxX) / rocketIdeal.maxX * 100:F1}%");
            Console.WriteLine($"   • Высоту на {(rocketIdeal.maxY - maxY) / rocketIdeal.maxY * 100:F1}%");
            Console.WriteLine($"   • Время полета на {(rocketIdeal.maxTime - maxTime) / rocketIdeal.maxTime * 100:F1}%");
        }

        // ------------------------------------------------------------
        // ВЫВОД СПРАВКИ
        // ------------------------------------------------------------
        public static void PrintHelp()
        {
            Console.WriteLine(new string('─', 70));
            Console.WriteLine("         ПРОГРАММА ДЛЯ РАСЧЕТА ПОЛЕТА ТЕЛА");
            Console.WriteLine(new string('─', 70));
            Console.WriteLine("\n ОПИСАНИЕ:");
            Console.WriteLine("   Программа рассчитывает траекторию тела, брошенного под углом,");
            Console.WriteLine("   с возможностью учета сопротивления воздуха.");
            Console.WriteLine("\n ВХОДНЫЕ ПАРАМЕТРЫ:");
            Console.WriteLine("   ┌─────────────────────────────────────────────────────────────────┐");
            Console.WriteLine("   │ a  - угол броска (0-90 градусов)                                │");
            Console.WriteLine("   │ t  - максимальное время расчета (секунды)                       │");
            Console.WriteLine("   │ v0 - начальная скорость (м/с)                                   │");
            Console.WriteLine("   │ h  - начальная высота (метры)                                   │");
            Console.WriteLine("   │ C  - коэффициент сопротивления (0 - нет, 0.1-1.0 - есть)        │");
            Console.WriteLine("   │ m  - масса объекта (кг)                                         │");
            Console.WriteLine("   │ A  - площадь поперечного сечения (м²)                           │");
            Console.WriteLine("   └─────────────────────────────────────────────────────────────────┘");
            Console.WriteLine("\n  ФИЗИЧЕСКИЕ КОНСТАНТЫ:");
            Console.WriteLine($"   • Ускорение свободного падения: g = {G} м/с²");
            Console.WriteLine($"   • Плотность воздуха: ρ = {AirDensity} кг/м³ (на уровне моря)");
            Console.WriteLine("\n ПРИМЕРЫ ЗНАЧЕНИЙ:");
            Console.WriteLine("   • Пуля:        C ≈ 0.1,  m = 0.01 кг, A = 0.0001 м²");
            Console.WriteLine("   • Мяч:         C ≈ 0.5,  m = 0.4 кг,  A = 0.04 м²");
            Console.WriteLine("   • Камень:      C ≈ 0.8,  m = 1 кг,    A = 0.01 м²");
            Console.WriteLine("   • Лист бумаги: C ≈ 1.0,  m = 0.01 кг, A = 0.05 м²");
            Console.WriteLine(new string('=', 70));
            Console.WriteLine();
        }

        // ------------------------------------------------------------
        // ПОЛУЧЕНИЕ МАТРИЦЫ ТРАЕКТОРИИ ДЛЯ АНИМАЦИИ
        // ------------------------------------------------------------
        public double[,] GetTrajectoryMatrix()
        {
            return coordinatesMatrix;
        }
    }

    // ================================================================
    // ГЛАВНЫЙ КЛАСС ПРОГРАММЫ
    // ================================================================
    class Program
    {
        /*
        [STAThread]
        static void Main(string[] args)
        {
            // Настройка консоли для лучшего отображения
            Console.Title = " Симулятор полета ракеты";
            Console.ForegroundColor = ConsoleColor.White;

            // Выводим справку
            Rocket.PrintHelp();

            // ВВОД ПАРАМЕТРОВ ПОЛЬЗОВАТЕЛЕМ 
            double angle, maxTime, velocity, height;
            double dragCoefficient = 0;
            double mass = 1;
            double area = 0.1;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" ВВЕДИТЕ ПАРАМЕТРЫ ПОЛЕТА:");
            Console.ResetColor();
            Console.WriteLine(new string('─', 50));

            // Угол (градусы)
            Console.Write("   Угол броска a (0-90°): ");
            while (!double.TryParse(Console.ReadLine(), out angle) || angle < 0 || angle > 90)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("   ❌ Ошибка! Введите число от 0 до 90: ");
                Console.ResetColor();
            }

            // Время
            Console.Write("   Максимальное время t (секунды): ");
            while (!double.TryParse(Console.ReadLine(), out maxTime))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("   ❌ Ошибка! Введите положительное число: ");
                Console.ResetColor();
            }

            // Начальная скорость
            Console.Write("   Начальная скорость v0 (м/с): ");
            while (!double.TryParse(Console.ReadLine(), out velocity))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("   ❌ Ошибка! Введите положительное число: ");
                Console.ResetColor();
            }

            // Высота
            Console.Write("   Начальная высота h (метры): ");
            while (!double.TryParse(Console.ReadLine(), out height) || height < 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("   ❌ Ошибка! Введите неотрицательное число: ");
                Console.ResetColor();
            }

            // Коэффициент сопротивления
            Console.Write("   Коэффициент сопротивления C (0-1.0, 0 = без воздуха): ");
            while (!double.TryParse(Console.ReadLine(), out dragCoefficient) || dragCoefficient < 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("   ❌ Ошибка! Введите число >= 0: ");
                Console.ResetColor();
            }

            // Если есть сопротивление, запрашиваем массу и площадь
            if (dragCoefficient > 0)
            {
                Console.Write("   Масса объекта m (кг): ");
                while (!double.TryParse(Console.ReadLine(), out mass) || mass <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("   ❌ Ошибка! Введите положительное число: ");
                    Console.ResetColor();
                }

                Console.Write("   Площадь сечения A (м²): ");
                while (!double.TryParse(Console.ReadLine(), out area) || area <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("   ❌ Ошибка! Введите положительное число: ");
                    Console.ResetColor();
                }
            }

            Console.WriteLine(new string('─', 50));

            //  РАСЧЕТ 
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n ВЫПОЛНЯЕТСЯ РАСЧЕТ ТРАЕКТОРИИ...");
            Console.ResetColor();

            // Создаем объект ракеты с заданными параметрами
            Rocket rocket = new Rocket(angle, maxTime, velocity, height, dragCoefficient, mass, area);

            // Подписываемся на событие падения
            rocket.RocketFall += (isFallen) =>
            {
                if (isFallen)
                    Console.WriteLine("    Ракета достигла поверхности земли!");
                else
                    Console.WriteLine("    Ракета не упала за указанное время!");
            };

            // Запускаем расчет траектории
            rocket.CalculateTrajectory();

            // Выводим результаты в консоль
            rocket.PrintTable();
            rocket.DrawTrajectory();

            // Сравнение с идеальным полетом (если есть сопротивление)
            if (dragCoefficient > 0)
            {
                rocket.CompareWithNoResistance();
            }

            //  ГРАФИЧЕСКИЕ ОКНА 
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n Нажмите любую клавишу для открытия окон с результатами и анимацией...");
            Console.ResetColor();
            Console.ReadKey();

            // Получаем матрицу траектории
            double[,] trajectoryMatrix = rocket.GetTrajectoryMatrix();

            // Создаем и показываем оба окна
            if (System.Windows.Application.Current == null)
            {
                // Создаем приложение в отдельном потоке
                var thread = new System.Threading.Thread(() =>
                {
                    Application app = new Application();

                    // Создаем окна
                    Window resultWindow = new Window();
                    WinRocket.WinRocketTool(rocket.maxTime, rocket.maxX, rocket.maxY);
                    RocketAnimationWindow animationWindow = new RocketAnimationWindow(trajectoryMatrix, rocket.maxX, rocket.maxY, rocket.maxTime);
                    animationWindow.Show();

                    app.Run();
                });
                thread.SetApartmentState(System.Threading.ApartmentState.STA);
                thread.Start();
            }
            else
            {
                // Если приложение уже существует, просто открываем окна
                WinRocket.WinRocketTool(rocket.maxTime, rocket.maxX, rocket.maxY);
                RocketAnimationWindow animationWindow = new RocketAnimationWindow(trajectoryMatrix, rocket.maxX, rocket.maxY, rocket.maxTime);
                animationWindow.Show();
            }
        }
        */

        [STAThread]
        static void Main(string[] args)
        {
            // Создаем окно ввода
            InputDialog inputDialog = new InputDialog();

            // Показываем окно ввода и ждем результат
            bool? result = inputDialog.ShowDialog();

            // Если пользователь нажал OK
            if (result == true && inputDialog.IsConfirmed)
            {
                // Получаем параметры из окна ввода
                double angle = inputDialog.InputAngle;
                double maxTime = inputDialog.InputTime;
                double velocity = inputDialog.InputVelocity;
                double height = inputDialog.InputHeight;
                double dragCoefficient = inputDialog.InputDrag;
                double mass = inputDialog.InputMass;
                double area = inputDialog.InputArea;

                // Создаем ракету и выполняем расчет
                Rocket rocket = new Rocket(angle, maxTime, velocity, height, dragCoefficient, mass, area);
                rocket.CalculateTrajectory();

                // Вывод в консоль
                Console.Title = "Симулятор полета ракеты";
                Console.ForegroundColor = ConsoleColor.White;
                Rocket.PrintHelp();
                Console.WriteLine($"\nПараметры: угол={angle}°, время={maxTime}с, скорость={velocity}м/с, высота={height}м");
                if (dragCoefficient > 0)
                    Console.WriteLine($"Сопротивление: C={dragCoefficient}, m={mass}кг, A={area}м²");
                Console.WriteLine("\nВЫПОЛНЯЕТСЯ РАСЧЕТ...");
                rocket.PrintTable();
                rocket.DrawTrajectory();
                if (dragCoefficient > 0)
                    rocket.CompareWithNoResistance();

                Console.WriteLine("\nНажмите любую клавишу для открытия анимации...");
                Console.ReadKey();

                // Запускаем анимацию в отдельном потоке
                var animationThread = new System.Threading.Thread(() =>
                {
                    Application app = new Application();

                    // Создаем и показываем окна
                    WinRocket.WinRocketTool(rocket.maxTime, rocket.maxX, rocket.maxY);
                    RocketAnimationWindow animationWindow = new RocketAnimationWindow(
                        rocket.GetTrajectoryMatrix(), rocket.maxX, rocket.maxY, rocket.maxTime);
                    animationWindow.Show();

                    app.Run();
                });

                animationThread.SetApartmentState(System.Threading.ApartmentState.STA);
                animationThread.Start();
                animationThread.Join();
            }
            else
            {
                Console.WriteLine("Программа завершена.");
                Console.ReadKey();
            }
        }
    }
}