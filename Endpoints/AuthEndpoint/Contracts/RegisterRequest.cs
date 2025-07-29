﻿namespace Peyghoom.Endpoints.AuthEndpoint.Contracts;

public class RegisterRequest
{
   public required string UserName { get; set; }
   public required string FirstName { get; set; }
   public required string LastName { get; set; }
}