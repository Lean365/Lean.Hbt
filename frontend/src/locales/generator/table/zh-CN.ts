export default {
  generator: {
    table: {
      title: '代码生成',
      list: {
        title: '代码生成列表',
        search: {
          name: '表名',
          comment: '表注释'
        },
        table: {
          tableId: '表ID',
          databaseName: '数据库名',
          tableName: '表名',
          tableComment: '表注释',
          className: '类名',
          namespace: '命名空间',
          baseNamespace: '基础命名空间',
          csharpTypeName: 'C#类型名',
          parentTableName: '父表名',
          parentTableFkName: '父表外键',
          status: '状态',
          templateType: '模板类型',
          moduleName: '模块名',
          businessName: '业务名',
          functionName: '功能名',
          author: '作者',
          genMode: '生成模式',
          genPath: '生成路径',
          options: '选项',
          tenantId: '租户ID',
          createBy: '创建者',
          createTime: '创建时间',
          updateBy: '更新者',
          updateTime: '更新时间',
          remark: '备注',
          isDeleted: '是否删除'
        },
        actions: {
          create: '新增',
          edit: '修改',
          delete: '删除',
          view: '查看',
          generate: '生成代码',
          sync: '同步表',
          import: '导入',
          export: '导出',
          template: '下载模板',
          refresh: '刷新'
        },
        status: {
          enabled: '启用',
          disabled: '停用'
        }
      },
      form: {
        title: '代码生成表单',
        tab: {
          basic: '基本信息',
          generate: '生成信息',
          field: '字段信息'
        },
        basic: {
          title: '基本信息',
          tableName: '表名',
          tableComment: '表注释',
          className: '类名',
          namespace: '命名空间',
          baseNamespace: '基础命名空间',
          csharpTypeName: 'C#类型名',
          parentTableName: '父表名',
          parentTableFkName: '父表外键',
          status: '状态',
          author: '作者',
          remark: '备注'
        },
        generate: {
          title: '生成信息',
          moduleName: '模块名',
          packageName: '包路径',
          businessName: '业务名',
          functionName: '功能名',
          parentMenuId: '上级菜单',
          tplCategory: '模板类型',
          genPath: '生成路径',
          options: '生成选项',
          tplCategoryOptions: {
            crud: '单表（增删改查）',
            tree: '树表（增删改查）',
            sub: '主子表（增删改查）'
          },
          optionsItems: {
            treeCode: '树编码字段',
            treeParentCode: '树父编码字段',
            treeName: '树名称字段',
            parentMenuId: '上级菜单',
            query: '查询',
            add: '新增',
            edit: '修改',
            delete: '删除',
            import: '导入',
            export: '导出'
          }
        },
        field: {
          title: '字段信息',
          columnName: '列名',
          columnComment: '列注释',
          columnType: '列类型',
          csharpType: 'C#类型',
          csharpField: 'C#字段',
          isRequired: '必填',
          isInsert: '插入',
          isEdit: '编辑',
          isList: '列表',
          isQuery: '查询',
          queryType: '查询类型',
          htmlType: '显示类型',
          dictType: '字典类型',
          queryTypeOptions: {
            EQ: '等于',
            NE: '不等于',
            GT: '大于',
            GTE: '大于等于',
            LT: '小于',
            LTE: '小于等于',
            LIKE: '模糊',
            BETWEEN: '范围',
            IN: '包含'
          },
          htmlTypeOptions: {
            input: '输入框',
            textarea: '文本域',
            select: '下拉框',
            radio: '单选框',
            checkbox: '复选框',
            datetime: '日期时间',
            imageUpload: '图片上传',
            fileUpload: '文件上传',
            editor: '富文本编辑器'
          }
        },
        buttons: {
          submit: '提交',
          cancel: '取消'
        },
        name: '表名',
        comment: '表注释',
        className: '类名',
        namespace: '命名空间',
        baseNamespace: '基础命名空间',
        csharpTypeName: 'C#类型名',
        parentTableName: '父表名',
        parentTableFkName: '父表外键',
        moduleName: '模块名',
        businessName: '业务名',
        functionName: '功能名',
        author: '作者',
        genMode: '生成模式',
        genPath: '生成路径',
        options: '选项'
      },
      detail: {
        title: '代码生成详情',
        basic: {
          title: '基本信息',
          tableName: '表名',
          tableComment: '表注释',
          className: '类名',
          namespace: '命名空间',
          baseNamespace: '基础命名空间',
          csharpTypeName: 'C#类型名',
          parentTableName: '父表名',
          parentTableFkName: '父表外键',
          status: '状态',
          createTime: '创建时间',
          updateTime: '更新时间'
        },
        generate: {
          title: '生成信息',
          moduleName: '模块名',
          packageName: '包路径',
          businessName: '业务名',
          functionName: '功能名',
          parentMenuId: '上级菜单',
          tplCategory: '模板类型',
          genPath: '生成路径',
          options: '生成选项'
        },
        field: {
          title: '字段信息',
          columnName: '列名',
          columnComment: '列注释',
          columnType: '列类型',
          csharpType: 'C#类型',
          csharpField: 'C#字段',
          isRequired: '必填',
          isInsert: '插入',
          isEdit: '编辑',
          isList: '列表',
          isQuery: '查询',
          queryType: '查询类型',
          htmlType: '显示类型',
          dictType: '字典类型'
        },
        actions: {
          edit: '修改',
          back: '返回'
        },
        columnInfo: '列信息',
        javaType: 'Java类型',
        javaField: 'Java字段',
        yes: '是',
        no: '否'
      },
      name: '表名',
      comment: '表注释',
      databaseName: '数据库名',
      className: '类名',
      namespace: '命名空间',
      baseNamespace: '基础命名空间',
      csharpTypeName: 'C#类型名',
      parentTableName: '父表名',
      parentTableFkName: '父表外键',
      status: '状态',
      templateType: '模板类型',
      moduleName: '模块名',
      businessName: '业务名',
      functionName: '功能名',
      author: '作者',
      genMode: '生成模式',
      genPath: '生成路径',
      options: '选项',
      tenantId: '租户ID',
      createBy: '创建者',
      createTime: '创建时间',
      updateBy: '更新者',
      updateTime: '更新时间',
      remark: '备注',
      isDeleted: '是否删除',
      placeholder: {
        name: '请输入表名',
        comment: '请输入表注释'
      },
      preview: {
        title: '代码预览',
        copy: '复制代码',
        download: '下载代码',
        showLineNumbers: '显示行号',
        hideLineNumbers: '隐藏行号',
        copySuccess: '复制成功',
        copyFailed: '复制失败',
        downloadSuccess: '下载成功',
        downloadFailed: '下载失败',
        tab: {
          java: 'Java代码',
          vue: 'Vue代码',
          sql: 'SQL代码',
          domain: '实体类',
          mapper: 'Mapper接口',
          mapperXml: 'Mapper XML',
          service: '服务接口',
          serviceImpl: '服务实现',
          controller: '控制器',
          api: 'API接口',
          index: '列表页面',
          form: '表单页面'
        },
        entities: {
          title: '实体类代码'
        },
        services: {
          title: '服务接口代码'
        },
        controllers: {
          title: '控制器代码'
        },
        vue: {
          title: 'Vue代码'
        },
        dtos: {
          title: '数据传输对象代码'
        },
        types: {
          title: '类型定义代码'
        },
        locales: {
          title: '国际化代码'
        }
      },
      import: {
        title: '导入表',
        database: '数据库',
        table: {
          name: '表名',
          comment: '表注释',
          action: '操作'
        },
        column: {
          title: '导入列',
          tableName: '表名',
          tableId: '表ID',
          columnName: '列名',
          propertyName: '属性名',
          columnType: '列类型',
          propertyType: '属性类型',
          isNullable: '允许空值',
          isPrimaryKey: '主键',
          isAutoIncrement: '自增',
          defaultValue: '默认值',
          columnComment: '列注释',
          value: '值',
          decimalDigits: '小数位数',
          scale: '精度',
          isArray: '数组',
          isJson: 'Json',
          isUnsigned: '无符号',
          createTableFieldSort: '创建表字段排序',
          insertServerTime: '插入服务器时间',
          insertSql: '插入SQL',
          updateServerTime: '更新服务器时间',
          updateSql: '更新SQL',
          sqlParameterDbType: 'SQL参数数据库类型'
        }
      },
      message: {
        generateSuccess: '生成代码成功',
        generateFailed: '生成代码失败',
        syncSuccess: '同步表成功',
        syncFailed: '同步表失败',
        importSuccess: '导入成功',
        importFailed: '导入失败',
        exportSuccess: '导出成功',
        exportFailed: '导出失败',
        templateSuccess: '下载模板成功',
        templateFailed: '下载模板失败',
        selectDatabase: '请先选择数据库',
        selectTable: '请选择要导入的表',
        tableNameRequired: '表名不能为空',
        importTimeout: '导入超时，请稍后重试'
      },
      tab: {
        basic: '基本信息',
        generate: '生成信息',
        field: '字段信息'
      },
      required: {
        name: '请输入表名',
        comment: '请输入表注释',
        className: '请输入类名',
        namespace: '请输入命名空间',
        baseNamespace: '请输入基础命名空间',
        csharpTypeName: '请输入C#类型名',
        moduleName: '请输入模块名',
        businessName: '请输入业务名',
        functionName: '请输入功能名',
        author: '请输入作者',
        genMode: '请选择生成模式',
        genPath: '请输入生成路径'
      }
    }
  }
} 