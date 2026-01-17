using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Common.Models
{
    /// <summary>
    /// Patrón Result para retornar exitos o fallos en operaciones sin sacar excepciones.
    /// Usado para fallos esperados en lógica de negocio (No technical errors).
    /// </summary>
    /// <typeparam name="T">El tipo del valor retornado cuando la operación es exitosa.</typeparam>
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public T Value { get; }
        public string Error { get; }
        public List<string> Errors { get; }

        private Result(bool isSuccess, T value, string error, List<string> errors = null)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
            Errors = errors ?? new List<string>();
        }

        /// <summary>
        /// Crea un resultado exitoso con el valor proporcionado.
        /// </summary>
        /// <param name="value">Valor</param>
        /// <returns></returns>
        public static Result<T> Success(T value)
        {
            return new Result<T>(true, value, null);
        }

        /// <summary>
        /// Crea un resultado fallido con el mensaje de error proporcionado.
        /// </summary>
        /// <param name="error">Mensaje de error</param>
        /// <returns></returns>
        public static Result<T> Failure(string error)
        {
            return new Result<T>(false, default, error);
        }

        /// <summary>
        /// Crea un resultado fallido con una lista de mensajes de error.
        /// </summary>
        /// <param name="errors">Lista de errores</param>
        /// <returns></returns>
        public static Result<T> Failure(List<string> errors)
        {
            return new Result<T>(false, default, errors.FirstOrDefault(), errors);
        }

        /// <summary>
        /// Crea un resultado fallido a partir de una excepción.
        /// </summary>
        /// <param name="exception">Excepción</param>
        /// <returns></returns>
        public static Result<T> Failure(Exception exception)
        {
            return new Result<T>(false, default, exception.Message);
        }
    }

    /// <summary>
    /// Patrón Result para retornar exitos o fallos en operaciones que no retornan un valor.
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; }
        public List<string> Errors { get; }

        private Result(bool isSuccess, string error, List<string> errors = null)
        {
            IsSuccess = isSuccess;
            Error = error;
            Errors = errors ?? new List<string>();
        }

        /// <summary>
        /// Crea un resultado exitoso.
        /// </summary>
        /// <returns></returns>
        public static Result Success()
        {
            return new Result(true, null);
        }

        /// <summary>
        /// Crea un resultado fallido con el mensaje de error proporcionado.
        /// </summary>
        /// <param name="error">Mensaje de error</param>
        /// <returns></returns>
        public static Result Failure(string error)
        {
            return new Result(false, error);
        }

        /// <summary>
        /// Crea un resultado fallido con una lista de mensajes de error.
        /// </summary>
        /// <param name="errors">Lista de errores</param>
        /// <returns></returns>
        public static Result Failure(List<string> errors)
        {
            return new Result(false, errors.FirstOrDefault(), errors);
        }

        /// <summary>
        /// Crea un resultado fallido a partir de una excepción.
        /// </summary>
        /// <param name="exception">Excepción</param>
        /// <returns></returns>
        public static Result Failure(Exception exception)
        {
            return new Result(false, exception.Message);
        }

        /// <summary>
        /// Convierte este resultado en un Result<T>, proporcionando un valor si es exitoso.
        /// </summary>
        /// <typeparam name="T">El tipo de response</typeparam>
        /// <param name="value">Valor</param>
        /// <returns></returns>
        public Result<T> ToResult<T>(T value = default) => 
            IsSuccess
                ? Result<T>.Success(value) 
                : Result<T>.Failure(Error);
    }
}
