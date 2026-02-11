-- 1. Създаване на таблица с преподаватели
CREATE TABLE teachers (
    teacher_id INTEGER PRIMARY KEY,
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    subject TEXT NOT NULL,
    email TEXT,
    phone TEXT
);

-- 2. Създаване на таблица с предмети
CREATE TABLE subjects (
    subject_id INTEGER PRIMARY KEY,
    name TEXT NOT NULL,
    teacher_id INTEGER,
    FOREIGN KEY (teacher_id) REFERENCES teachers(teacher_id)
);

-- 3. Създаване на таблица с ученици
CREATE TABLE students (
    student_id INTEGER PRIMARY KEY,
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    egn TEXT UNIQUE NOT NULL,
    birth_date DATE,
    class TEXT,
    teacher_id INTEGER,
    FOREIGN KEY (teacher_id) REFERENCES teachers(teacher_id)
);

-- 4. Създаване на таблица с оценки
CREATE TABLE grades (
    grade_id INTEGER PRIMARY KEY,
    student_id INTEGER,
    subject_id INTEGER,
    grade_date DATE,
    grade_type TEXT,
    grade_value INTEGER,
    FOREIGN KEY (student_id) REFERENCES students(student_id),
    FOREIGN KEY (subject_id) REFERENCES subjects(subject_id)
);

-- 5. Създаване на таблица с отсъствия
CREATE TABLE absences (
    absence_id INTEGER PRIMARY KEY,
    student_id INTEGER,
    subject_id INTEGER,
    absence_date DATE,
    FOREIGN KEY (student_id) REFERENCES students(student_id),
    FOREIGN KEY (subject_id) REFERENCES subjects(subject_id)
);

-- Вмъкване на преподаватели
INSERT INTO teachers VALUES
(1, 'Иван', 'Петров', 'Математика', 'ivan@school.bg', '0888123456'),
(2, 'Мария', 'Иванова', 'Български език', 'maria@school.bg', '0888765432'),
(3, 'Георги', 'Димитров', 'История', 'georgi@school.bg', '0888123123'),
(4, 'Надя', 'Кирова', 'Физика', 'nadya@school.bg', '0888345678'),
(5, 'Даниел', 'Атанасов', 'Химия', 'daniel@school.bg', '0888456789'),
(6, 'Елена', 'Попова', 'Английски език', 'elena@school.bg', '0888123987'),
(7, 'Симеон', 'Василев', 'География', 'simeon@school.bg', '0888456123'),
(8, 'Петя', 'Стефанова', 'Информатика', 'petya@school.bg', '0888567890'),
(9, 'Росен', 'Божинов', 'Биология', 'rosen@school.bg', '0888123678'),
(10, 'Цветелина', 'Жекова', 'Музика', 'tsveta@school.bg', '0888765433');

-- Вмъкване на предмети
INSERT INTO subjects VALUES
(1, 'Математика', 1),
(2, 'Български език', 2),
(3, 'История', 3),
(4, 'Физика', 4),
(5, 'Химия', 5),
(6, 'Английски език', 6),
(7, 'География', 7),
(8, 'Информатика', 8),
(9, 'Биология', 9),
(10, 'Музика', 10);

-- Вмъкване на ученици
INSERT INTO students VALUES
(1, 'Анна', 'Тодорова', '0301011234', '2009-03-01', '6А', 1),
(2, 'Борис', 'Георгиев', '0301021235', '2009-04-15', '6А', 2),
(3, 'Виктория', 'Иванова', '0301031236', '2009-05-20', '6Б', 3),
(4, 'Георги', 'Петров', '0301041237', '2009-06-10', '6Б', 4),
(5, 'Даниела', 'Николова', '0301051238', '2009-07-12', '6А', 5),
(6, 'Емил', 'Костов', '0301061239', '2009-08-14', '6А', 6),
(7, 'Живко', 'Михайлов', '0301071240', '2009-09-18', '6Б', 7),
(8, 'Зорница', 'Славова', '0301081241', '2009-10-20', '6А', 8),
(9, 'Илия', 'Тодоров', '0301091242', '2009-11-22', '6Б', 9),
(10, 'Калоян', 'Христов', '0301101243', '2009-12-25', '6А', 10);

-- Вмъкване на оценки
INSERT INTO grades VALUES
(1, 1, 1, '2024-01-15', 'контролно', 5),
(2, 2, 2, '2024-01-20', 'изпитване', 4),
(3, 3, 3, '2024-01-25', 'тест', 6),
(4, 4, 4, '2024-02-01', 'контролно', 3),
(5, 5, 5, '2024-02-05', 'домашно', 6),
(6, 6, 6, '2024-02-10', 'тест', 5),
(7, 7, 7, '2024-02-15', 'контролно', 4),
(8, 8, 8, '2024-02-20', 'изпитване', 5),
(9, 9, 9, '2024-02-25', 'контролно', 6),
(10, 10, 10, '2024-03-01', 'тест', 5);

-- Вмъкване на отсъствия
INSERT INTO absences VALUES
(1, 1, 1, '2024-01-12', 1),
(2, 2, 2, '2024-01-18', 0),
(3, 3, 3, '2024-01-24', 1),
(4, 4, 4, '2024-02-02', 0),
(5, 5, 5, '2024-02-06', 1),
(6, 6, 6, '2024-02-11', 1),
(7, 7, 7, '2024-02-16', 0),
(8, 8, 8, '2024-02-21', 1),
(9, 9, 9, '2024-02-26', 0),
(10, 10, 10, '2024-03-02', 1);


-- queries.sql

-- 1. Списък с всички ученици по класове, сортирани по фамилия
SELECT class, last_name, first_name
FROM students
ORDER BY class, last_name;

-- 2. Справка за оценки по предмет и по ученик за избран период
SELECT students.first_name, students.last_name, subjects.name, grades.grade_value, grades.grade_date
FROM students, subjects, grades
WHERE students.student_id = grades.student_id
AND subjects.subject_id = grades.subject_id
AND grades.grade_date BETWEEN '2024-01-01' AND '2024-03-31';

-- 3. Справка за общия брой оценки за всеки ученик
SELECT students.first_name, students.last_name, COUNT(grades.grade_id) AS total_grades
FROM students, grades
WHERE students.student_id = grades.student_id
GROUP BY students.student_id, students.first_name, students.last_name;

-- 4. Среден брой оценки за учениците по клас
-- Забележка: Първо броим оценките на ученик, после осредняваме по клас
SELECT class, AVG(grade_count) AS avg_grades_per_class
FROM (
    SELECT class, students.student_id, COUNT(grade_id) AS grade_count
    FROM students, grades
    WHERE students.student_id = grades.student_id
    GROUP BY students.student_id, class
)
GROUP BY class;

-- 5. Минимална и максимална оценка за конкретен ученик (напр. Анна Тодорова)
SELECT MIN(grade_value) AS min_grade, MAX(grade_value) AS max_grade
FROM grades, students
WHERE grades.student_id = students.student_id
AND students.first_name = 'Анна' AND students.last_name = 'Тодорова';

-- 6. Общо отсъствия по клас
SELECT students.class, COUNT(absences.absence_id) AS total_absences
FROM students, absences
WHERE students.student_id = absences.student_id
GROUP BY students.class;

-- 7. Списък на учениците с повече от 3 отсъствия
-- (В текущите данни няма такива, но заявката е структурно правилна)
SELECT students.first_name, students.last_name, COUNT(absences.absence_id) AS absence_count
FROM students, absences
WHERE students.student_id = absences.student_id
GROUP BY students.student_id, students.first_name, students.last_name
HAVING COUNT(absences.absence_id) > 3;

-- 8. Оценка по предмет за конкретен ученик за месец Януари
SELECT students.first_name, subjects.name, grades.grade_value, grades.grade_date
FROM students, subjects, grades
WHERE students.student_id = grades.student_id
AND subjects.subject_id = grades.subject_id
AND students.first_name = 'Анна'
AND strftime('%m', grades.grade_date) = '01';

-- 9. Списък на преподавателите по броя на предметите, които преподават
SELECT teachers.first_name, teachers.last_name, COUNT(subjects.subject_id) AS subjects_taught
FROM teachers, subjects
WHERE teachers.teacher_id = subjects.teacher_id
GROUP BY teachers.teacher_id, teachers.first_name, teachers.last_name;

-- 10. Изчисляване на средна оценка по предмет за всички ученици
SELECT subjects.name, AVG(grades.grade_value) AS average_grade
FROM subjects, grades
WHERE subjects.subject_id = grades.subject_id
GROUP BY subjects.name;

-- 11. Обновяване на оценка за ученик по даден предмет
UPDATE grades
SET grade_value = 6
WHERE student_id = (SELECT student_id FROM students WHERE first_name = 'Борис' AND last_name = 'Георгиев')
AND subject_id = (SELECT subject_id FROM subjects WHERE name = 'Български език')
AND grade_date = '2024-01-20';

-- 12. Изтриване на отсъствие за конкретен ученик
DELETE FROM absences
WHERE student_id = (SELECT student_id FROM students WHERE first_name = 'Анна')
AND absence_date = '2024-01-12';

-- 13. Броене на учениците по класове
SELECT class, COUNT(student_id) AS student_count
FROM students
GROUP BY class;

-- 14. Групиране на учениците по клас и сортиране по дата на раждане
SELECT class, first_name, last_name, birth_date
FROM students
ORDER BY class, birth_date;

-- 15. Преброяване на учениците, които са отсъствали повече от 5 пъти
SELECT COUNT(*) AS high_absence_students_count
FROM (
    SELECT student_id
    FROM absences
    GROUP BY student_id
    HAVING COUNT(absence_id) > 5
);

-- 16. Изтриване на всички оценки за ученик, който е отпаднал от курса (напр. Калоян Христов)
DELETE FROM grades
WHERE student_id = (SELECT student_id FROM students WHERE first_name = 'Калоян' AND last_name = 'Христов');

-- 17. Изчисляване на средния брой отсъствия на учениците по класове
SELECT class, AVG(abs_count) AS avg_absences
FROM (
    SELECT class, students.student_id, COUNT(absence_id) AS abs_count
    FROM students, absences
    WHERE students.student_id = absences.student_id
    GROUP BY students.student_id, class
)
GROUP BY class;

-- 18. Да се демонстрира конкатенация (Име и Фамилия в една колона)
-- В SQLite се използва || за конкатенация
SELECT first_name || ' ' || last_name AS full_name
FROM students;

-- 19. Филтриране на учениците по месец/година на раждане (напр. Родени през март)
SELECT first_name, last_name, birth_date
FROM students
WHERE strftime('%m', birth_date) = '03';

-- 20. Да се съкрати името на ученик до първите две букви
-- Използваме SUBSTR (аналог на MID/LEFT в други системи)
SELECT SUBSTR(first_name, 1, 2) AS short_name, last_name
FROM students;