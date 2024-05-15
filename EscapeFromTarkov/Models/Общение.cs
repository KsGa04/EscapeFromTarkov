using System;
using System.Collections.Generic;

namespace EscapeFromTarkov.Models
{
    public partial class Общение
    {
        public int ОбщениеId { get; set; }
        public string Сообщение { get; set; } = null!;
        public int? Отправитель { get; set; }
        public int? Получатель { get; set; }
        public DateTime? ВремяОтправки { get; set; }

        public virtual Пользователь? ОтправительNavigation { get; set; }
        public virtual Пользователь? ПолучательNavigation { get; set; }
    }
}
