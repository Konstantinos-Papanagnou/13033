namespace _13033.Model
{
    /// <summary>
    /// Model of the message to send
    /// </summary>
    public class Model
    {
        public int Code { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        /// <summary>
        /// Confirm that all the inputs are correct
        /// </summary>
        /// <returns>true if everything checks out</returns>
        public bool ConfirmMessage()
        {
            if (Code == 0)
                return false;
            if (string.IsNullOrEmpty(Surname))
                return false;
            if (string.IsNullOrEmpty(Name))
                return false;
            if (string.IsNullOrEmpty(Address))
                return false;
            return true;
        }
        /// <summary>
        /// Get the final message
        /// </summary>
        /// <returns>The completed message to be send</returns>
        public string GetMessage()
        {
            return Code.ToString() + " " + Surname + " " + Name + " " + Address;
        }

        public Model(int Code, string Surname, string Name, string Address)
        {
            this.Code = Code;
            this.Surname = Surname;
            this.Name = Name;
            this.Address = Address;
        }
    }
}