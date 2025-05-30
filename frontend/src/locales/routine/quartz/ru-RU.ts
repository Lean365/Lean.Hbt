export default {
  routine: {
    quartz: {
      // Основная информация
      jobId: 'ID задачи',
      jobName: 'Название задачи',
      jobGroup: 'Группа задач',
      jobClass: 'Класс задачи',
      jobMethod: 'Метод задачи',
      jobParams: 'Параметры задачи',
      cronExpression: 'Cron-выражение',
      jobStatus: 'Статус задачи',
      remark: 'Примечание',
      createTime: 'Время создания',
      updateTime: 'Время обновления',

      // Кнопки действий
      add: 'Добавить задачу',
      edit: 'Редактировать задачу',
      delete: 'Удалить задачу',
      batchDelete: 'Пакетное удаление',
      export: 'Экспорт',
      import: 'Импорт',
      downloadTemplate: 'Скачать шаблон',
      preview: 'Предпросмотр',
      execute: 'Выполнить',
      pause: 'Приостановить',
      resume: 'Возобновить',

      // Плейсхолдеры формы
      placeholder: {
        jobId: 'Введите ID задачи',
        jobName: 'Введите название задачи',
        jobGroup: 'Введите группу задач',
        jobClass: 'Введите класс задачи',
        jobMethod: 'Введите метод задачи',
        jobParams: 'Введите параметры задачи',
        cronExpression: 'Введите Cron-выражение',
        jobStatus: 'Выберите статус задачи',
        remark: 'Введите примечание',
        startTime: 'Время начала',
        endTime: 'Время окончания'
      },

      // Валидация формы
      validation: {
        jobId: {
          required: 'Введите ID задачи',
          maxLength: 'ID задачи не может превышать 100 символов'
        },
        jobName: {
          required: 'Введите название задачи',
          maxLength: 'Название задачи не может превышать 50 символов'
        },
        jobGroup: {
          required: 'Введите группу задач',
          maxLength: 'Группа задач не может превышать 50 символов'
        },
        jobClass: {
          required: 'Введите класс задачи',
          maxLength: 'Класс задачи не может превышать 200 символов'
        },
        jobMethod: {
          required: 'Введите метод задачи',
          maxLength: 'Метод задачи не может превышать 100 символов'
        },
        cronExpression: {
          required: 'Введите Cron-выражение',
          maxLength: 'Cron-выражение не может превышать 100 символов'
        },
        jobStatus: {
          required: 'Выберите статус задачи'
        }
      },

      // Результаты операций
      message: {
        success: {
          add: 'Успешно добавлено',
          edit: 'Успешно отредактировано',
          delete: 'Успешно удалено',
          batchDelete: 'Успешно удалено пакетно',
          export: 'Успешно экспортировано',
          import: 'Успешно импортировано',
          execute: 'Успешно выполнено',
          pause: 'Успешно приостановлено',
          resume: 'Успешно возобновлено'
        },
        failed: {
          add: 'Ошибка добавления',
          edit: 'Ошибка редактирования',
          delete: 'Ошибка удаления',
          batchDelete: 'Ошибка пакетного удаления',
          export: 'Ошибка экспорта',
          import: 'Ошибка импорта',
          execute: 'Ошибка выполнения',
          pause: 'Ошибка приостановки',
          resume: 'Ошибка возобновления'
        }
      },

      // Страница деталей
      detail: {
        title: 'Детали задачи'
      }
    }
  }
} 