using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data;
using Npgsql;
using PrototypeWebBlockchain.Models;
using System.IO;
using System.Security.Cryptography;
using System;
using System.Web;
using System.Text;
using PrototypeWebBlockchain.Functions.Default;
using System.Web.Mvc;

namespace PrototypeWebBlockchain.Repository
{
    public class MemberFunction : DefaultWeb3
    {
        public Member CreateAccount(Member member)
        {
            var web3 = InitializeWeb3();

            var taskCreateAccount = web3.Personal.NewAccount.SendRequestAsync(member.password);
            taskCreateAccount.Wait();

            member.w_address = taskCreateAccount.Result;

            return member;
        }

        public ModelStateDictionary Validate(Member _member, MemberRepository _memberRepository)
        {
            var memberlist = _memberRepository.FindAll();
            var modelstate = new ModelStateDictionary();

            var count = memberlist.Where(r => r.email == _member.email).LongCount();
            if(count != 0)
            {
                 modelstate.AddModelError("email", "Email is Duplicated");
            }

            if(_member.password != _member.cpassword)
            {
                modelstate.AddModelError("password", "Password did not match");
                modelstate.AddModelError("cpassword", "Password did not match");
            }

            return modelstate;

        }
    }
}