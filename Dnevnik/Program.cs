// ООП И БАЗИ ДАННИ
// --------------------------------------------------------------------------------
// 1. КЛАСОВЕ И ОБЕКТИ: Класът е шаблон (Person), а обектите са конкретните инстанции (Student, Teacher).
// 2. ЕНКАПСУЛАЦИЯ: Използване на свойства (get; set;) за контролиран достъп до данните.
// 3. НАСЛЕДЯВАНЕ (Inheritance): Подкласовете поемат характеристиките на базовия клас (Person -> Student).
// 4. ПОЛИМОРФИЗМ (Polymorphism): Пренаписване на методи (override), така че един метод да действа различно.
// 5. АБСТРАКЦИЯ: Скриване на сложните SQL детайли зад прости бутони и методи в интерфейса.
// 6. ПЪРВИЧЕН КЛЮЧ (Primary Key): Уникален идентификатор за всеки запис в базата (ID).
// --------------------------------------------------------------------------------

// Program.cs
// dotnet add package System.Data.OleDb
// accessdatabaseengine_X64
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Linq;

// Базов клас "Човек"
public class Person
{
    // Свойства (Properties) за съхранение на данни
    public int ID { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";

    // Конструктор: Метод за инициализиране на обекта при създаването му с 'new'
    public Person(int id, string firstName, string lastName)
    {
        ID = id;
        FirstName = firstName;
        LastName = lastName;
    }

    // Виртуален метод: Позволява на наследниците да го променят (полиморфизъм)
    public virtual string GetInfo() => $"{FirstName} {LastName}";

    // Пренаписване на системния метод ToString за нуждите на ListBox
    public override string ToString() => GetInfo();
}

// Наследник "Учител"
public class Teacher : Person
{
    public string Subject { get; set; } = "";

    // Извикване на конструктора на базовия клас чрез 'base'
    public Teacher(int id, string firstName, string lastName, string subject)
        : base(id, firstName, lastName) => Subject = subject;

    // Полиморфизъм: Промяна на поведението на GetInfo за учители
    public override string GetInfo() => $"[УЧИТЕЛ] {base.GetInfo()} - {Subject}";
}

// Наследник "Ученик"
public class Student : Person
{
    public string ClassName { get; set; } = "";
    public string EGN { get; set; } = "";

    public Student(int id, string firstName, string lastName, string className, string egn = "")
        : base(id, firstName, lastName)
    {
        ClassName = className;
        EGN = egn;
    }

    // Полиморфизъм: Промяна на поведението на GetInfo за ученици
    public override string GetInfo() => $"[УЧЕНИК] {base.GetInfo()} ({ClassName})";
}

// Главен клас за интерфейса, наследяващ системния клас Form
public class ProgramForm : Form
{
    // Компоненти на потребителския интерфейс (Controls)
    private ListBox lstData = new ListBox();
    private Button btnLoad = new Button();
    private Button btnDelete = new Button();
    private Button btnAdd = new Button();
    private Button btnEdit = new Button();
    private Button btnSearch = new Button();
    private Button btnAddTeacher = new Button();
    private Button btnClear = new Button();
    private Button btnExport = new Button();

    private TextBox txtFirstName = new TextBox();
    private TextBox txtLastName = new TextBox();
    private TextBox txtClass = new TextBox();
    private TextBox txtSubject = new TextBox();
    private TextBox txtSearch = new TextBox();
    private Label lblStatus = new Label();
    private Label lblCount = new Label();
    private Label lblFirstName = new Label();
    private Label lblLastName = new Label();
    private Label lblClass = new Label();
    private Label lblSubject = new Label();

    private ComboBox cmbFilterType = new ComboBox();

    // Път до базата и Connection String за връзка с Access
    private string dbPath = Path.Combine(Application.StartupPath, "dnevnik.accdb");
    private string ConnString => $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};";

    // Конструктор на формата: Настройка на визуалните елементи
    public ProgramForm()
    {
        this.Text = "Училищен Дневник - ООП и Бази Данни";
        this.Size = new System.Drawing.Size(950, 650);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Позициониране на елементите върху екрана
        lstData.Bounds = new System.Drawing.Rectangle(20, 70, 550, 400);
        lstData.Font = new System.Drawing.Font("Consolas", 10);
        // Абониране за събитие (Event Handling) за двоен клик
        lstData.DoubleClick += (s, e) => LoadSelectedForEdit();

        // Филтър комбо бокс
        cmbFilterType.Bounds = new System.Drawing.Rectangle(20, 20, 150, 30);
        cmbFilterType.Items.AddRange(new object[] { "Всички", "Учители", "Ученици" });
        cmbFilterType.SelectedIndex = 0;
        cmbFilterType.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbFilterType.SelectedIndexChanged += (s, e) => FilterData();

        // Търсене
        txtSearch.Bounds = new System.Drawing.Rectangle(180, 20, 250, 30);
        txtSearch.PlaceholderText = "Търсене по име...";
        txtSearch.TextChanged += (s, e) => FilterData();

        btnSearch.Bounds = new System.Drawing.Rectangle(440, 20, 130, 30);
        btnSearch.Text = "ИЗЧИСТИ";
        btnSearch.Click += (s, e) => { txtSearch.Clear(); cmbFilterType.SelectedIndex = 0; };

        // Бутони за управление
        btnLoad.Bounds = new System.Drawing.Rectangle(20, 480, 130, 45);
        btnLoad.Text = "ЗАРЕДИ";
        btnLoad.BackColor = System.Drawing.Color.LightBlue;
        // Абониране за събитие (Event Handling) чрез ламбда израз
        btnLoad.Click += (s, e) => RefreshAll();

        btnDelete.Bounds = new System.Drawing.Rectangle(160, 480, 130, 45);
        btnDelete.Text = "ИЗТРИЙ";
        btnDelete.BackColor = System.Drawing.Color.LightCoral;
        btnDelete.Click += (s, e) => DeleteSelected();

        btnEdit.Bounds = new System.Drawing.Rectangle(300, 480, 130, 45);
        btnEdit.Text = "РЕДАКТИРАЙ";
        btnEdit.BackColor = System.Drawing.Color.LightGoldenrodYellow;
        btnEdit.Click += (s, e) => LoadSelectedForEdit();

        btnExport.Bounds = new System.Drawing.Rectangle(440, 480, 130, 45);
        btnExport.Text = "ЕКСПОРТ";
        btnExport.BackColor = System.Drawing.Color.LightGreen;
        btnExport.Click += (s, e) => ExportToCSV();

        // Брой записи
        lblCount.Bounds = new System.Drawing.Rectangle(20, 535, 550, 25);
        lblCount.Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold);

        // Панел за добавяне/редакция
        lblFirstName.Bounds = new System.Drawing.Rectangle(600, 70, 300, 20);
        lblFirstName.Text = "Име:";

        txtFirstName.Bounds = new System.Drawing.Rectangle(600, 95, 300, 30);
        txtFirstName.Font = new System.Drawing.Font("Arial", 11);

        lblLastName.Bounds = new System.Drawing.Rectangle(600, 135, 300, 20);
        lblLastName.Text = "Фамилия:";

        txtLastName.Bounds = new System.Drawing.Rectangle(600, 160, 300, 30);
        txtLastName.Font = new System.Drawing.Font("Arial", 11);

        lblClass.Bounds = new System.Drawing.Rectangle(600, 200, 300, 20);
        lblClass.Text = "Клас (за ученик):";

        txtClass.Bounds = new System.Drawing.Rectangle(600, 225, 300, 30);
        txtClass.Font = new System.Drawing.Font("Arial", 11);

        lblSubject.Bounds = new System.Drawing.Rectangle(600, 265, 300, 20);
        lblSubject.Text = "Предмет (за учител):";

        txtSubject.Bounds = new System.Drawing.Rectangle(600, 290, 300, 30);
        txtSubject.Font = new System.Drawing.Font("Arial", 11);

        btnAdd.Bounds = new System.Drawing.Rectangle(600, 340, 300, 50);
        btnAdd.Text = "ДОБАВИ УЧЕНИК";
        btnAdd.BackColor = System.Drawing.Color.PaleGreen;
        btnAdd.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
        btnAdd.Click += (s, e) => AddStudent();

        btnAddTeacher.Bounds = new System.Drawing.Rectangle(600, 400, 300, 50);
        btnAddTeacher.Text = "ДОБАВИ УЧИТЕЛ";
        btnAddTeacher.BackColor = System.Drawing.Color.LightSteelBlue;
        btnAddTeacher.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
        btnAddTeacher.Click += (s, e) => AddTeacher();

        btnClear.Bounds = new System.Drawing.Rectangle(600, 460, 300, 40);
        btnClear.Text = "ИЗЧИСТИ ПОЛЕТА";
        btnClear.BackColor = System.Drawing.Color.WhiteSmoke;
        btnClear.Click += (s, e) => ClearFields();

        lblStatus.Bounds = new System.Drawing.Rectangle(20, 570, 880, 25);
        lblStatus.Text = "База данни: " + dbPath;
        lblStatus.Font = new System.Drawing.Font("Arial", 8);

        // Добавяне на контролите към формата
        this.Controls.AddRange(new Control[] {
            lstData, btnLoad, btnDelete, btnEdit, btnExport,
            txtFirstName, txtLastName, txtClass, txtSubject, txtSearch,
            btnAdd, btnAddTeacher, btnClear,
            lblStatus, lblCount, lblFirstName, lblLastName, lblClass, lblSubject,
            cmbFilterType, btnSearch
        });

        // Автоматично зареждане при стартиране
        this.Load += (s, e) => RefreshAll();
    }

    // Метод за извличане на данни (Четене от база данни)
    private void RefreshAll()
    {
        if (!File.Exists(dbPath))
        {
            MessageBox.Show("Базата не е намерена в директорията на програмата!\n\nПоставете файла dnevnik.accdb в папката с програмата.",
                "Грешка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        lstData.Items.Clear();
        using var conn = new OleDbConnection(ConnString);
        try
        {
            conn.Open(); // Отваряне на връзката

            // SQL заявка за учители
            var cmdT = new OleDbCommand("SELECT teacher_id, first_name, last_name, subject FROM teachers ORDER BY last_name", conn);
            using var rT = cmdT.ExecuteReader();
            while (rT.Read())
                lstData.Items.Add(new Teacher((int)rT[0], rT[1].ToString() ?? "", rT[2].ToString() ?? "", rT[3].ToString() ?? ""));

            // SQL заявка за ученици
            var cmdS = new OleDbCommand("SELECT student_id, first_name, last_name, class, egn FROM students ORDER BY last_name", conn);
            using var rS = cmdS.ExecuteReader();
            while (rS.Read())
                lstData.Items.Add(new Student((int)rS[0], rS[1].ToString() ?? "", rS[2].ToString() ?? "", rS[3].ToString() ?? "", rS[4]?.ToString() ?? ""));

            UpdateCount();
            lblStatus.Text = $"База данни: {dbPath} | Последно зареждане: {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Грешка при четене:\n\n{ex.Message}\n\nУверете се, че базата данни има таблици 'teachers' и 'students' с правилните колони.",
                "Грешка при четене", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Метод за записване на ученик (Добавяне в база данни)
    private void AddStudent()
    {
        if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
        {
            MessageBox.Show("Моля, въведете име и фамилия!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var conn = new OleDbConnection(ConnString);
        try
        {
            conn.Open();

            // Ръчно генериране на нов Първичен Ключ (Primary Key)
            int nextId = 1;
            var cmdId = new OleDbCommand("SELECT MAX(student_id) FROM students", conn);
            var result = cmdId.ExecuteScalar();
            if (result != DBNull.Value && result != null) nextId = Convert.ToInt32(result) + 1;

            // Използване на параметризирана SQL заявка за сигурност
            string sql = "INSERT INTO students (student_id, first_name, last_name, class, egn) VALUES (?, ?, ?, ?, ?)";
            using var cmd = new OleDbCommand(sql, conn);
            cmd.Parameters.AddWithValue("?", nextId);
            cmd.Parameters.AddWithValue("?", txtFirstName.Text.Trim());
            cmd.Parameters.AddWithValue("?", txtLastName.Text.Trim());
            cmd.Parameters.AddWithValue("?", string.IsNullOrWhiteSpace(txtClass.Text) ? "Не е указан" : txtClass.Text.Trim());
            cmd.Parameters.AddWithValue("?", DateTime.Now.Ticks.ToString().Substring(0, 10));

            cmd.ExecuteNonQuery(); // Изпълнение на записа
            RefreshAll(); // Опресняване на списъка
            ClearFields();
            MessageBox.Show($"Ученикът {txtFirstName.Text} {txtLastName.Text} беше добавен успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Грешка при запис на ученик:\n\n{ex.Message}", "Грешка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Метод за записване на учител (Добавяне в база данни)
    private void AddTeacher()
    {
        if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
        {
            MessageBox.Show("Моля, въведете име и фамилия!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var conn = new OleDbConnection(ConnString);
        try
        {
            conn.Open();

            // Ръчно генериране на нов Първичен Ключ (Primary Key)
            int nextId = 1;
            var cmdId = new OleDbCommand("SELECT MAX(teacher_id) FROM teachers", conn);
            var result = cmdId.ExecuteScalar();
            if (result != DBNull.Value && result != null) nextId = Convert.ToInt32(result) + 1;

            // Използване на параметризирана SQL заявка за сигурност
            string sql = "INSERT INTO teachers (teacher_id, first_name, last_name, subject) VALUES (?, ?, ?, ?)";
            using var cmd = new OleDbCommand(sql, conn);
            cmd.Parameters.AddWithValue("?", nextId);
            cmd.Parameters.AddWithValue("?", txtFirstName.Text.Trim());
            cmd.Parameters.AddWithValue("?", txtLastName.Text.Trim());
            cmd.Parameters.AddWithValue("?", string.IsNullOrWhiteSpace(txtSubject.Text) ? "Не е указан" : txtSubject.Text.Trim());

            cmd.ExecuteNonQuery(); // Изпълнение на записа
            RefreshAll(); // Опресняване на списъка
            ClearFields();
            MessageBox.Show($"Учителят {txtFirstName.Text} {txtLastName.Text} беше добавен успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Грешка при запис на учител:\n\n{ex.Message}", "Грешка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Метод за премахване (Изтриване от база данни)
    private void DeleteSelected()
    {
        // Проверка дали е избран обект и кастване към базовия тип Person
        if (lstData.SelectedItem is not Person p)
        {
            MessageBox.Show("Моля, изберете запис за изтриване!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Потвърждение преди изтриване
        var result = MessageBox.Show($"Сигурни ли сте, че искате да изтриете:\n\n{p.GetInfo()}?",
            "Потвърждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result != DialogResult.Yes) return;

        // Динамично определяне на таблицата според типа на обекта
        string table = (p is Teacher) ? "teachers" : "students";
        string idCol = (p is Teacher) ? "teacher_id" : "student_id";

        using var conn = new OleDbConnection(ConnString);
        try
        {
            conn.Open();
            using var cmd = new OleDbCommand($"DELETE FROM {table} WHERE {idCol} = ?", conn);
            cmd.Parameters.AddWithValue("?", p.ID);
            cmd.ExecuteNonQuery();
            RefreshAll();
            MessageBox.Show("Записът беше изтрит успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Грешка при изтриване:\n\n{ex.Message}", "Грешка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Метод за зареждане на избрания запис за редакция
    private void LoadSelectedForEdit()
    {
        if (lstData.SelectedItem is not Person p)
        {
            MessageBox.Show("Моля, изберете запис за редактиране!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        txtFirstName.Text = p.FirstName;
        txtLastName.Text = p.LastName;

        if (p is Teacher t)
        {
            txtSubject.Text = t.Subject;
            txtClass.Text = "";
        }
        else if (p is Student s)
        {
            txtClass.Text = s.ClassName;
            txtSubject.Text = "";
        }

        // Показване на диалог за редакция
        var editResult = MessageBox.Show(
            $"Редактирате: {p.GetInfo()}\n\nПромените полетата и натиснете OK за запазване.",
            "Редакция",
            MessageBoxButtons.OKCancel,
            MessageBoxIcon.Information);

        if (editResult == DialogResult.OK)
        {
            UpdateRecord(p);
        }
    }

    // Метод за актуализиране на запис в базата данни
    private void UpdateRecord(Person p)
    {
        if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
        {
            MessageBox.Show("Моля, въведете име и фамилия!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var conn = new OleDbConnection(ConnString);
        try
        {
            conn.Open();

            if (p is Teacher)
            {
                string sql = "UPDATE teachers SET first_name = ?, last_name = ?, subject = ? WHERE teacher_id = ?";
                using var cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddWithValue("?", txtFirstName.Text.Trim());
                cmd.Parameters.AddWithValue("?", txtLastName.Text.Trim());
                cmd.Parameters.AddWithValue("?", txtSubject.Text.Trim());
                cmd.Parameters.AddWithValue("?", p.ID);
                cmd.ExecuteNonQuery();
            }
            else if (p is Student)
            {
                string sql = "UPDATE students SET first_name = ?, last_name = ?, class = ? WHERE student_id = ?";
                using var cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddWithValue("?", txtFirstName.Text.Trim());
                cmd.Parameters.AddWithValue("?", txtLastName.Text.Trim());
                cmd.Parameters.AddWithValue("?", txtClass.Text.Trim());
                cmd.Parameters.AddWithValue("?", p.ID);
                cmd.ExecuteNonQuery();
            }

            RefreshAll();
            ClearFields();
            MessageBox.Show("Записът беше актуализиран успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Грешка при актуализация:\n\n{ex.Message}", "Грешка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Метод за изчистване на полетата за въвеждане
    private void ClearFields()
    {
        txtFirstName.Clear();
        txtLastName.Clear();
        txtClass.Clear();
        txtSubject.Clear();
        txtFirstName.Focus();
    }

    // Метод за актуализиране на броя записи
    private void UpdateCount()
    {
        int teachers = lstData.Items.OfType<Teacher>().Count();
        int students = lstData.Items.OfType<Student>().Count();
        lblCount.Text = $"Общо записи: {lstData.Items.Count} (Учители: {teachers}, Ученици: {students})";
    }

    // Метод за филтриране на данните
    private void FilterData()
    {
        if (!File.Exists(dbPath)) return;

        lstData.Items.Clear();
        using var conn = new OleDbConnection(ConnString);
        try
        {
            conn.Open();

            string searchText = txtSearch.Text.Trim().ToLower();
            int filterType = cmbFilterType.SelectedIndex;

            // Зареждане на учители (ако не е избран филтър само за ученици)
            if (filterType == 0 || filterType == 1)
            {
                var cmdT = new OleDbCommand("SELECT teacher_id, first_name, last_name, subject FROM teachers ORDER BY last_name", conn);
                using var rT = cmdT.ExecuteReader();
                while (rT.Read())
                {
                    var teacher = new Teacher((int)rT[0], rT[1].ToString() ?? "", rT[2].ToString() ?? "", rT[3].ToString() ?? "");
                    if (string.IsNullOrEmpty(searchText) ||
                        teacher.FirstName.ToLower().Contains(searchText) ||
                        teacher.LastName.ToLower().Contains(searchText) ||
                        teacher.Subject.ToLower().Contains(searchText))
                    {
                        lstData.Items.Add(teacher);
                    }
                }
            }

            // Зареждане на ученици (ако не е избран филтър само за учители)
            if (filterType == 0 || filterType == 2)
            {
                var cmdS = new OleDbCommand("SELECT student_id, first_name, last_name, class, egn FROM students ORDER BY last_name", conn);
                using var rS = cmdS.ExecuteReader();
                while (rS.Read())
                {
                    var student = new Student((int)rS[0], rS[1].ToString() ?? "", rS[2].ToString() ?? "", rS[3].ToString() ?? "", rS[4]?.ToString() ?? "");
                    if (string.IsNullOrEmpty(searchText) ||
                        student.FirstName.ToLower().Contains(searchText) ||
                        student.LastName.ToLower().Contains(searchText) ||
                        student.ClassName.ToLower().Contains(searchText))
                    {
                        lstData.Items.Add(student);
                    }
                }
            }

            UpdateCount();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Грешка при филтриране:\n\n{ex.Message}", "Грешка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Метод за експортиране на данните в CSV формат
    private void ExportToCSV()
    {
        if (lstData.Items.Count == 0)
        {
            MessageBox.Show("Няма данни за експортиране!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var dialog = new SaveFileDialog();
        dialog.Filter = "CSV файл|*.csv|Текстови файл|*.txt";
        dialog.FileName = $"dnevnik_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                using var writer = new StreamWriter(dialog.FileName, false, System.Text.Encoding.UTF8);
                writer.WriteLine("Тип;Име;Фамилия;Клас/Предмет");

                foreach (var item in lstData.Items)
                {
                    if (item is Teacher t)
                    {
                        writer.WriteLine($"Учител;{t.FirstName};{t.LastName};{t.Subject}");
                    }
                    else if (item is Student s)
                    {
                        writer.WriteLine($"Ученик;{s.FirstName};{s.LastName};{s.ClassName}");
                    }
                }

                MessageBox.Show($"Данните бяха експортирани успешно в:\n\n{dialog.FileName}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Грешка при експортиране:\n\n{ex.Message}", "Грешка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // Входна точка на приложението
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new ProgramForm());
    }
}